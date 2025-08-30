using FluentResults;

namespace LBB.Core.Contracts;

public interface IEmailSender
{
    Task<Result> SendEmail(string destEmail, string subject, string message);
}
