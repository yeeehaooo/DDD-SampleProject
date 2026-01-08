using SampleProject.Application.DTOs;
using SampleProject.Application.Mediator;

namespace SampleProject.Application.Queries.Product;

public record GetAllProductsQuery() : IRequest<IEnumerable<ProductDto>>;
