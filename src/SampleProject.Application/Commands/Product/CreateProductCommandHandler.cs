using Microsoft.Extensions.Logging;
using SampleProject.Application.DTOs;
using SampleProject.Application.Mediator;
using SampleProject.Domain.Interfaces;

namespace SampleProject.Application.Commands.Product;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _repository;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(
        IProductRepository repository,
        ILogger<CreateProductCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<ProductDto> HandleAsync(
        CreateProductCommand request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating product with name: {Name}", request.Name);

        var product = new Domain.Entities.Product(
            request.Name,
            request.Description,
            request.BasePrice);

        await _repository.AddAsync(product, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Product created with ProductId: {ProductId}", product.ProductId);

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
