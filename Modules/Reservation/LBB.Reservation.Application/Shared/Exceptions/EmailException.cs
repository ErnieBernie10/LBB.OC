using Microsoft.Extensions.Localization;
using OrchardCore.Email;

namespace LBB.Reservation.Application.Shared.Exceptions;

public class EmailException(EmailResult result) : Exception
{
    public EmailResult Result { get; } = result;

    public override string Message =>
        $"Email failed to send. Errors: {string.Join("\n", Result.Errors.Select(e => $"{e.Key}: {string.Join(", ", e.Value.Select(f => f.Value))}"))}";
}
