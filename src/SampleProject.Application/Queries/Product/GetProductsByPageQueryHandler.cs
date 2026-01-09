using SampleProject.Application.DTOs;
using SampleProject.Application.Mediator;
using SampleProject.Domain.Interfaces;

namespace SampleProject.Application.Queries.Product;

public class GetProductsByPageQueryHandler : IRequestHandler<GetProductsByPageQuery, PagedResultDto<ProductDto>>
{
    private readonly IProductRepository _repository;

    public GetProductsByPageQueryHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResultDto<ProductDto>> HandleAsync(
        GetProductsByPageQuery request,
        CancellationToken cancellationToken = default)
    {
        var products = await _repository.GetByPageAsync(
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var allProducts = await _repository.GetAllAsync(cancellationToken);
        var totalCount = allProducts.Count();
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var productDtos = products.Select(p => new ProductDto(
            p.Id,
            p.ProductId,
            p.Name.Value, // 從 ProductName 提取原始值
            p.Description,
            p.BasePrice.Amount, // 從 Money 提取原始值
            p.CreatedAt,
            p.UpdatedAt)).ToList();

        return new PagedResultDto<ProductDto>(
            productDtos,
            totalCount,
            request.PageNumber,
            request.PageSize,
            totalPages);
    }
}
