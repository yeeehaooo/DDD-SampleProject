using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace SampleProject.Application.Mediator;

public class Mediator : IMediator
{
    private static readonly ConcurrentDictionary<string, Type> HandlerTypeCache = new();
    private static readonly ConcurrentDictionary<Type, MethodInfo> MethodInfoCache = new();

    private readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> SendAsync<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(request);

        var requestType = request.GetType();
        var responseType = typeof(TResponse);
        var cacheKey = $"{requestType.FullName}|{responseType.FullName}";

        var handlerType = HandlerTypeCache.GetOrAdd(
            cacheKey,
            _ => typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType)
        );

        var handler = _serviceProvider.GetRequiredService(handlerType);

        var method = MethodInfoCache.GetOrAdd(
            handlerType,
            type =>
            {
                var methodInfo = type.GetMethod(
                    nameof(IRequestHandler<IRequest<object>, object>.HandleAsync),
                    BindingFlags.Instance | BindingFlags.Public
                );

                if (methodInfo == null)
                {
                    throw new InvalidOperationException(
                        $"Handler type {type.Name} does not implement HandleAsync method."
                    );
                }

                return methodInfo;
            }
        );

        // 建立 Handler 委派
        async Task<TResponse> HandlerDelegate()
        {
            try
            {
                var result = method.Invoke(handler, new object[] { request, cancellationToken });

                if (result is not Task<TResponse> task)
                {
                    throw new InvalidOperationException(
                        $"Handler for {requestType.Name} did not return a Task<{typeof(TResponse).Name}>."
                    );
                }

                return await task;
            }
            catch (TargetInvocationException ex) when (ex.InnerException != null)
            {
                throw ex.InnerException;
            }
        }

        // 取得所有 Pipeline Behaviors（使用反射取得正確的泛型類型）
        var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, responseType);
        var behaviors = _serviceProvider.GetServices(behaviorType).ToList();

        // 如果沒有 Behaviors，直接執行 Handler
        if (behaviors.Count == 0)
        {
            return await HandlerDelegate();
        }

        // 使用反射建立 Pipeline 委派鏈
        var handleMethod = behaviorType.GetMethod(
            nameof(IPipelineBehavior<IRequest<object>, object>.HandleAsync),
            new[]
            {
                requestType,
                typeof(RequestHandlerDelegate<TResponse>),
                typeof(CancellationToken),
            }
        )!;

        RequestHandlerDelegate<TResponse> pipeline = HandlerDelegate;

        // 從最後一個 Behavior 開始，向前建立委派鏈
        for (var i = behaviors.Count - 1; i >= 0; i--)
        {
            var behavior = behaviors[i];
            var next = pipeline;
            var capturedRequest = request;

            pipeline = async () =>
            {
                var result = handleMethod.Invoke(
                    behavior,
                    new object[] { capturedRequest, next, cancellationToken }
                )!;
                return await (Task<TResponse>)result;
            };
        }

        return await pipeline();
    }

    public Task SendAsync(IRequest request, CancellationToken cancellationToken = default)
    {
        return SendAsync<Unit>(request, cancellationToken);
    }
}
