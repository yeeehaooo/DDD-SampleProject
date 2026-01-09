using Microsoft.Extensions.Logging;
using SampleProject.Application.Mediator;

namespace SampleProject.Application.Mediator.Behaviors;

/// <summary>
/// Logging Pipeline Behavior
///
/// 統一記錄每個 Request 的執行時間、成功/失敗等資訊
/// </summary>
public class LoggingPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingPipelineBehavior<TRequest, TResponse>> _logger;

    public LoggingPipelineBehavior(ILogger<LoggingPipelineBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        var requestName = typeof(TRequest).Name;
        var requestId = Guid.NewGuid();

        _logger.LogInformation(
            "[{RequestId}] Starting {RequestName} at {StartTime:yyyy-MM-dd HH:mm:ss.fff}",
            requestId,
            requestName,
            DateTime.UtcNow);

        var startTime = DateTime.UtcNow;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            var response = await next();
            stopwatch.Stop();

            _logger.LogInformation(
                "[{RequestId}] Completed {RequestName} successfully in {ElapsedMilliseconds}ms",
                requestId,
                requestName,
                stopwatch.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(
                ex,
                "[{RequestId}] Failed {RequestName} after {ElapsedMilliseconds}ms: {ErrorMessage}",
                requestId,
                requestName,
                stopwatch.ElapsedMilliseconds,
                ex.Message);

            throw;
        }
    }
}
