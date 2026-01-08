using SampleProject.Application.DTOs;
using SampleProject.Application.Mediator;
using SampleProject.Domain.Interfaces;

namespace SampleProject.Application.Queries.Inventory;

public class GetInventoryByStorageIdQueryHandler : IRequestHandler<GetInventoryByStorageIdQuery, List<InventoryDto>>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ISkuRepository _skuRepository;
    private readonly IStorageRepository _storageRepository;

    public GetInventoryByStorageIdQueryHandler(
        IInventoryRepository inventoryRepository,
        ISkuRepository skuRepository,
        IStorageRepository storageRepository)
    {
        _inventoryRepository = inventoryRepository;
        _skuRepository = skuRepository;
        _storageRepository = storageRepository;
    }

    public async Task<List<InventoryDto>> HandleAsync(
        GetInventoryByStorageIdQuery request,
        CancellationToken cancellationToken = default)
    {
        var storage = await _storageRepository.GetByStorageIdAsync(request.StorageId, cancellationToken);
        if (storage == null)
        {
            throw new KeyNotFoundException($"Storage with StorageId {request.StorageId} not found");
        }

        var inventories = await _inventoryRepository.GetByStorageIdAsync(storage.Id, cancellationToken);
        var result = new List<InventoryDto>();

        foreach (var inventory in inventories)
        {
            var sku = await _skuRepository.GetByIdAsync(inventory.SkuId, cancellationToken);
            if (sku != null)
            {
                result.Add(new InventoryDto(
                    sku.Id,
                    sku.SkuCode,
                    storage.Id,
                    storage.Name,
                    inventory.Quantity,
                    inventory.UpdatedAt));
            }
        }

        return result;
    }
}
