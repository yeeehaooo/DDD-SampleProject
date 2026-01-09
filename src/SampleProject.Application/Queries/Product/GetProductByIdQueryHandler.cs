using SampleProject.Application.DTOs;
using SampleProject.Application.Mediator;
using SampleProject.Domain.Interfaces;

namespace SampleProject.Application.Queries.Product;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IProductRepository _repository;

    public GetProductByIdQueryHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductDto?> HandleAsync(
        GetProductByIdQuery request,
        CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetByProductIdAsync(request.ProductId, cancellationToken);

        if (product == null)
            return null;

        return new ProductDto(
            product.Id,
            product.ProductId,
            product.Name.Value, // 從 ProductName 提取原始值
            product.Description,
            product.BasePrice.Amount, // 從 Money 提取原始值
            product.CreatedAt,
            product.UpdatedAt);
    }
}
