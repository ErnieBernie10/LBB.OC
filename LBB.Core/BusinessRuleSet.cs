using FluentResults;
using LBB.Core.Contracts;

namespace LBB.Core;

public class BusinessRuleSet<T>(params IBusinessRule<T>[] rules)
{
    public IEnumerable<IBusinessRule<T>> Rules { get; set; } = rules;

    public async Task<Result> ValidateAsync(T input, CancellationToken ct = default)
    {
        var failures = new List<IError>();

        foreach (var rule in Rules)
        {
            var result = await rule.CheckAsync(input, ct);
            if (result.IsFailed)
                failures.AddRange(result.Errors);
        }

        return failures.Any() ? Result.Fail(failures) : Result.Ok();
    }
}
