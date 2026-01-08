using Microsoft.Extensions.DependencyInjection;

namespace SampleProject.Application.Mediator;

public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var requestType = request.GetType();
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));
        var handler = _serviceProvider.GetRequiredService(handlerType);

        var method = handlerType.GetMethod(nameof(IRequestHandler<IRequest<object>, object>.HandleAsync))!;
        var result = method.Invoke(handler, new object[] { request, cancellationToken });

        return await (Task<TResponse>)result!;
    }

    public Task SendAsync(IRequest request, CancellationToken cancellationToken = default)
    {
        return SendAsync<Unit>(request, cancellationToken);
    }
}
