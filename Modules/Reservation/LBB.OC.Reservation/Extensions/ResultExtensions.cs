using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using FluentResults;
using LBB.Core.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LBB.OC.Reservation.Extensions;

public static class ResultExtensions
{
    public static ValidationProblemDetails? MapValidationErrorsToProblemDetails(
        this ResultBase result,
        HttpContext? httpContext = null,
        string? instance = null
    )
    {
        if (!result.IsFailed)
            return null;

        // Standard RFC 7807-compatible map: property -> messages[]
        var messages = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);

        // Rich details including metadata: property -> [{ message, code, metadata-flattened }]
        var detailed = new Dictionary<string, List<object>>(StringComparer.OrdinalIgnoreCase);

        foreach (var error in result.Errors)
        {
            // 1) Map DomainValidationError (preferred)
            if (error is DomainValidationError dve)
            {
                var key = string.IsNullOrWhiteSpace(dve.PropertyName)
                    ? string.Empty
                    : dve.PropertyName;

                // populate messages
                if (!messages.TryGetValue(key, out var arr))
                {
                    messages[key] = new[] { dve.Message };
                }
                else
                {
                    messages[key] = arr.Concat(new[] { dve.Message }).ToArray();
                }

                // populate detailed with metadata flattened as top-level properties
                if (!detailed.TryGetValue(key, out var list))
                {
                    list = new List<object>();
                    detailed[key] = list;
                }

                list.Add(
                    CreateFlatErrorObject(
                        propertyName: key,
                        message: dve.Message,
                        errorCode: dve.ErrorCode,
                        metadata: error.Metadata
                    )
                );

                continue;
            }

            // 2) Backward compatibility: map FluentValidation ValidationError wrapper
            if (error is ValidationError ve)
            {
                foreach (var validationError in ve.Result.Errors)
                {
                    var key = string.IsNullOrWhiteSpace(validationError.PropertyName)
                        ? string.Empty
                        : validationError.PropertyName;

                    // Populate messages map
                    if (!messages.TryGetValue(key, out var arr))
                    {
                        messages[key] = new[] { validationError.ErrorMessage };
                    }
                    else
                    {
                        messages[key] = arr.Concat(new[] { validationError.ErrorMessage })
                            .ToArray();
                    }

                    // Populate detailed map with metadata flattened (e.g., min/max for range validators)
                    if (!detailed.TryGetValue(key, out var list))
                    {
                        list = new List<object>();
                        detailed[key] = list;
                    }

                    // Build a metadata dictionary from FluentValidation's ValidationFailure
                    var meta = new Dictionary<string, object?>();
                    try
                    {
                        meta["AttemptedValue"] = validationError.AttemptedValue;
                        meta["Severity"] = validationError.Severity.ToString();
                        meta["CustomState"] = validationError.CustomState;
                        if (validationError.FormattedMessagePlaceholderValues != null)
                        {
                            foreach (var kv in validationError.FormattedMessagePlaceholderValues)
                            {
                                // Flatten placeholder values like MinLength, MaxLength, ComparisonValue, etc.
                                if (!meta.ContainsKey(kv.Key))
                                    meta[kv.Key] = kv.Value;
                            }
                        }
                    }
                    catch
                    {
                        // ignore metadata extraction failures; keep it best-effort
                    }

                    // Align with DomainValidationError flattened shape
                    list.Add(
                        CreateFlatErrorObject(
                            propertyName: key,
                            message: validationError.ErrorMessage,
                            errorCode: validationError.ErrorCode,
                            metadata: meta
                        )
                    );
                }
            }
        }

        if (messages.Count == 0)
            return null;

        var vpd = new ValidationProblemDetails(messages)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "One or more validation errors occurred.",
            Type = "https://datatracker.ietf.org/doc/html/rfc9110#name-400-bad-request",
            Instance = instance ?? httpContext?.Request?.Path.Value,
        };

        var traceId = httpContext?.TraceIdentifier;
        if (!string.IsNullOrEmpty(traceId))
        {
            vpd.Extensions["traceId"] = traceId;
        }

        // Add the rich, metadata-carrying structure
        if (detailed.Count > 0)
        {
            vpd.Extensions["errorsDetailed"] = detailed.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value
            );
        }

        return vpd;
    }

    private static Dictionary<string, object?> CreateFlatErrorObject(
        string propertyName,
        string message,
        string errorCode,
        IReadOnlyDictionary<string, object?>? metadata
    )
    {
        var dict = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
        {
            ["propertyName"] = propertyName,
            ["errorCode"] = errorCode,
            ["message"] = message,
        };

        if (metadata != null)
        {
            foreach (var kv in metadata)
            {
                var camelKey = ToCamelCase(kv.Key);
                // Avoid overriding core fields if collision occurs
                if (!dict.ContainsKey(camelKey))
                    dict[camelKey] = kv.Value;
            }
        }

        return dict;
    }

    private static string ToCamelCase(string key)
    {
        if (string.IsNullOrEmpty(key))
            return key;
        if (char.IsLower(key[0]))
            return key;
        if (key.Length == 1)
            return key.ToLowerInvariant();
        return char.ToLowerInvariant(key[0]) + key.Substring(1);
    }
}
