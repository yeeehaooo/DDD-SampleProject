using SampleProject.Application.Mediator;

namespace SampleProject.Application.Commands.Product;

public record DeleteProductCommand(Guid ProductId) : IRequest;
