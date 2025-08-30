using FluentResults;

namespace LBB.Core.Contracts;

public interface IValidator<in TObject>
    where TObject : class
{
    Task<Result> ValidateAsync(TObject obj, CancellationToken cancellationToken = default);
}
