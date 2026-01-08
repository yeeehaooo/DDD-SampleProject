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
            product.Name,
            product.Description,
            product.BasePrice,
            product.CreatedAt,
            product.UpdatedAt);
    }
}
