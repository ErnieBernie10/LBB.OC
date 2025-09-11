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

        // Rich details including metadata: property -> [{ message, code, metadata }]
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

                // populate detailed with any available metadata from FluentResults.Error
                if (!detailed.TryGetValue(key, out var list))
                {
                    list = new List<object>();
                    detailed[key] = list;
                }

                list.Add(dve);

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

                    // Populate detailed map with metadata (e.g., min/max for range validators)
                    if (!detailed.TryGetValue(key, out var list))
                    {
                        list = new List<object>();
                        detailed[key] = list;
                    }

                    list.Add(ve);
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
}
