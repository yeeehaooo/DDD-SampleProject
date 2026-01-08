using SampleProject.Application.DTOs;
using SampleProject.Application.Mediator;

namespace SampleProject.Application.Queries.Product;

public record GetProductsByPageQuery(
    int PageNumber,
    int PageSize
) : IRequest<PagedResultDto<ProductDto>>;
