namespace SampleProject.Application.Mediator;

/// <summary>
/// Pipeline Behavior 介面
///
/// 用於在 Handler 執行前後執行橫切關注點（如驗證、日誌、事務等）
/// </summary>
public interface IPipelineBehavior<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Handler 委派，代表下一個 Behavior 或實際的 Handler
/// </summary>
public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();
