namespace SampleProject.Application.Mediator;

public interface IRequest<out TResponse>
{
}

public interface IRequest : IRequest<Unit>
{
}

public record Unit;
