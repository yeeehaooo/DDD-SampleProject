using System.Reflection;
using SampleProject.Application.Mediator;
using SampleProject.Application.Mediator.Behaviors;

namespace SampleProject.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterRequestHandlers(this IServiceCollection services)
    {
        var assembly = Assembly.Load("SampleProject.Application");
        var handlerTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface &&
                t.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    (i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) ||
                     i.GetGenericTypeDefinition() == typeof(IRequestHandler<>))))
            .ToList();

        foreach (var handlerType in handlerTypes)
        {
            // 找出 Handler 直接實作的介面（不包括繼承的介面）
            var interfaces = handlerType.GetInterfaces()
                .Where(i => i.IsGenericType &&
                    (i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) ||
                     i.GetGenericTypeDefinition() == typeof(IRequestHandler<>)))
                .ToList();

            // 只註冊最直接的介面（如果實作 IRequestHandler<TRequest>，就不註冊 IRequestHandler<TRequest, Unit>）
            foreach (var interfaceType in interfaces)
            {
                // 檢查是否已經有其他介面是這個介面的父介面
                var isMostSpecific = !interfaces.Any(other =>
                    other != interfaceType &&
                    interfaceType.IsAssignableFrom(other));

                if (isMostSpecific)
                {
                    services.AddScoped(interfaceType, handlerType);
                }
            }
        }

        return services;
    }

    /// <summary>
    /// 註冊 Mediator Pipeline Behaviors
    ///
    /// 執行順序（從外到內）：
    /// 1. LoggingPipelineBehavior（最先執行，記錄開始時間）
    /// 2. ValidationPipelineBehavior（驗證請求）
    /// 3. TransactionPipelineBehavior（事務管理，預留）
    /// 4. Handler（實際業務邏輯）
    /// </summary>
    public static IServiceCollection RegisterPipelineBehaviors(this IServiceCollection services)
    {
        // 註冊 Logging Pipeline Behavior（第一個執行）
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));

        // 註冊 Validation Pipeline Behavior（第二個執行）
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));

        // 註冊 Transaction Pipeline Behavior（預留，目前不實作）
        // services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionPipelineBehavior<,>));

        return services;
    }
}
