using Microsoft.Extensions.Logging;
using SampleProject.Application.Mediator;

namespace SampleProject.Application.Mediator.Behaviors;

/// <summary>
/// Transaction Pipeline Behavior（預留，目前不實作）
///
/// 注意：由於目前 Dapper Repository 實作中，每個方法都建立獨立的 connection，
/// 無法在 Pipeline 層級統一管理事務。如需事務管理，有以下選項：
///
/// 1. 實作 UnitOfWork 模式：
///    - 建立 IUnitOfWork 介面，管理單一 connection 和 transaction
///    - Repository 接收 IUnitOfWork，共享同一個 connection
///    - 在 Handler 或 Pipeline 中管理 UnitOfWork 生命週期
///
/// 2. 在 Handler 層級管理事務：
///    - 對於需要事務的複雜操作，在 Handler 中手動管理
///    - 使用 IDbConnectionFactory 建立 connection，執行多個操作
///
/// 目前此 Behavior 為預留架構，暫不啟用。
/// </summary>
public class TransactionPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TransactionPipelineBehavior<TRequest, TResponse>> _logger;

    public TransactionPipelineBehavior(
        ILogger<TransactionPipelineBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        // 目前不實做事務管理，直接執行下一個 Behavior 或 Handler
        // 如需啟用，請先實作 UnitOfWork 模式
        return next();
    }
}
