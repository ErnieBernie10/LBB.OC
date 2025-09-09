using FluentResults;
using System.Net;
using System.Linq;
using FluentResults;
using LBB.Core.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace LBB.OC.Reservation.Extensions;

public static class ResultExtensions
{
    private sealed class ValidationErrorDetail
    {
        public string Message { get; set; } = default!;
        public string? Code { get; set; }
        public IDictionary<string, object?>? Metadata { get; set; }
    }

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
        var detailed = new Dictionary<string, List<ValidationErrorDetail>>(StringComparer.OrdinalIgnoreCase);

        foreach (var error in result.Errors)
        {
            if (error is ValidationError ve)
            {
                foreach (var validationError in ve.Result.Errors)
                {
                    var key = string.IsNullOrWhiteSpace(validationError.PropertyName)
                        ? string.Empty
                        : validationError.PropertyName;

                    // 1) Populate messages map
                    if (!messages.TryGetValue(key, out var arr))
                    {
                        messages[key] = new[] { validationError.ErrorMessage };
                    }
                    else
                    {
                        messages[key] = arr.Concat(new[] { validationError.ErrorMessage }).ToArray();
                    }

                    // 2) Populate detailed map with metadata (e.g., min/max for range validators)
                    if (!detailed.TryGetValue(key, out var list))
                    {
                        list = new List<ValidationErrorDetail>();
                        detailed[key] = list;
                    }

                    list.Add(new ValidationErrorDetail
                    {
                        Message = validationError.ErrorMessage,
                        Code = validationError.ErrorCode,
                        // This often contains keys like "From"/"To" for ranges, "MinLength"/"MaxLength" for length validators, etc.
                        Metadata = validationError.FormattedMessagePlaceholderValues
                            ?.ToDictionary(k => k.Key, v => v.Value)
                    });
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
            Instance = instance ?? httpContext?.Request?.Path.Value
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
                kvp => kvp.Value.Select(d => new
                {
                    message = d.Message,
                    code = d.Code,
                    metadata = d.Metadata
                }).ToArray()
            );
        }

        return vpd;
    }
}

