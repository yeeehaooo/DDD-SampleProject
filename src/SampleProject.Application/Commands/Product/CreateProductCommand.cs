using SampleProject.Application.DTOs;
using SampleProject.Application.Mediator;

namespace SampleProject.Application.Commands.Product;

public record CreateProductCommand(
    string Name,
    string Description,
    decimal BasePrice
) : IRequest<ProductDto>;
