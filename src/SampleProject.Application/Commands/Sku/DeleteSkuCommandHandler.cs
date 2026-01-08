using Microsoft.Extensions.Logging;
using SampleProject.Application.Mediator;
using SampleProject.Domain.Interfaces;

namespace SampleProject.Application.Commands.Sku;

public class DeleteSkuCommandHandler : IRequestHandler<DeleteSkuCommand>
{
    private readonly ISkuRepository _skuRepository;
    private readonly ILogger<DeleteSkuCommandHandler> _logger;

    public DeleteSkuCommandHandler(
        ISkuRepository skuRepository,
        ILogger<DeleteSkuCommandHandler> logger)
    {
        _skuRepository = skuRepository;
        _logger = logger;
    }

    public async Task<Unit> HandleAsync(
        DeleteSkuCommand request,
        CancellationToken cancellationToken = default)
    {
        var sku = await _skuRepository.GetBySkuIdAsync(request.SkuId, cancellationToken);
        if (sku == null)
        {
            throw new KeyNotFoundException($"Sku with SkuId {request.SkuId} not found");
        }

        await _skuRepository.DeleteAsync(sku.Id, cancellationToken);
        await _skuRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Sku deleted with SkuId: {SkuId}", request.SkuId);

        return new Unit();
    }
}
