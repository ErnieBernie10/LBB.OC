using FluentResults;

namespace LBB.Core.Contracts;

public interface IBusinessRule<in T>
{
    string Name { get; }
    Task<Result> CheckAsync(T input, CancellationToken ct = default);
}
