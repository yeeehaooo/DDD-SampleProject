using SampleProject.Application.DTOs;
using SampleProject.Application.Mediator;
using SampleProject.Domain.Interfaces;

namespace SampleProject.Application.Queries.Product;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<ProductDto>>
{
    private readonly IProductRepository _repository;

    public GetAllProductsQueryHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ProductDto>> HandleAsync(
        GetAllProductsQuery request,
        CancellationToken cancellationToken = default)
    {
        var products = await _repository.GetAllAsync(cancellationToken);

        // Repository 已經返回 List，這裡的 Select 轉換後也需要 ToList
        // 確保序列化時不會遇到迭代器問題
        return products.Select(p => new ProductDto(
            p.Id,
            p.ProductId,
            p.Name.Value, // 從 ProductName 提取原始值
            p.Description,
            p.BasePrice.Amount, // 從 Money 提取原始值
            p.CreatedAt,
            p.UpdatedAt)).ToList();
    }
}
