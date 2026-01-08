using System.Reflection;
using SampleProject.Application.Mediator;

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
}
