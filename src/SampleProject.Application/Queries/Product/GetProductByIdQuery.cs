using SampleProject.Application.DTOs;
using SampleProject.Application.Mediator;

namespace SampleProject.Application.Queries.Product;

public record GetProductByIdQuery(Guid ProductId) : IRequest<ProductDto?>;
