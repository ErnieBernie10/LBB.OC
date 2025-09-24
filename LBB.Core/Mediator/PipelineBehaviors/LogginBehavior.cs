using Microsoft.Extensions.Logging;

namespace LBB.Core.Mediator.PipelineBehaviors;

using System.Diagnostics;
using Microsoft.Extensions.Logging;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> HandleAsync(
        TRequest request,
        Func<Task<TResponse>> next,
        CancellationToken cancellationToken = default
    )
    {
        var requestName = typeof(TRequest).Name;
        var sw = Stopwatch.StartNew();

        try
        {
            var response = await next();
            sw.Stop();

            _logger.LogInformation(
                "{Request} handled in {Duration}ms",
                requestName,
                sw.ElapsedMilliseconds
            );

            return response;
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(
                ex,
                "{Request} failed after {Duration}ms",
                requestName,
                sw.ElapsedMilliseconds
            );
            throw;
        }
    }
}
