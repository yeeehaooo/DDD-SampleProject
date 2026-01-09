using Microsoft.Extensions.Logging;
using SampleProject.Application.DTOs;
using SampleProject.Application.Mediator;
using SampleProject.Domain.Interfaces;

namespace SampleProject.Application.Commands.Product;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
{
    private readonly IProductRepository _repository;
    private readonly ILogger<UpdateProductCommandHandler> _logger;

    public UpdateProductCommandHandler(
        IProductRepository repository,
        ILogger<UpdateProductCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<ProductDto> HandleAsync(
        UpdateProductCommand request,
        CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetByProductIdAsync(request.ProductId, cancellationToken);
        if (product == null)
        {
            throw new KeyNotFoundException($"Product with ProductId {request.ProductId} not found");
        }

        if (request.Name != null)
        {
            product.UpdateName(request.Name);
        }

        if (request.Description != null)
        {
            product.UpdateDescription(request.Description);
        }

        if (request.BasePrice.HasValue)
        {
            product.UpdateBasePrice(request.BasePrice.Value);
        }

        await _repository.UpdateAsync(product, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Product updated with ProductId: {ProductId}", product.ProductId);

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
