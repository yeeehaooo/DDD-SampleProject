using SampleProject.Application.DTOs;
using SampleProject.Application.Mediator;

namespace SampleProject.Application.Commands.Product;

public record UpdateProductCommand(
    Guid ProductId,
    string? Name,
    string? Description,
    decimal? BasePrice
) : IRequest<ProductDto>;
