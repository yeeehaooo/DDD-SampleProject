using Microsoft.Extensions.Logging;
using SampleProject.Application.Mediator;
using SampleProject.Domain.Interfaces;

namespace SampleProject.Application.Commands.Product;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly IProductRepository _repository;
    private readonly ILogger<DeleteProductCommandHandler> _logger;

    public DeleteProductCommandHandler(
        IProductRepository repository,
        ILogger<DeleteProductCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Unit> HandleAsync(
        DeleteProductCommand request,
        CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetByProductIdAsync(request.ProductId, cancellationToken);
        if (product == null)
        {
            throw new KeyNotFoundException($"Product with ProductId {request.ProductId} not found");
        }

        await _repository.DeleteAsync(product.Id, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Product deleted with ProductId: {ProductId}", request.ProductId);

        return new Unit();
    }
}
