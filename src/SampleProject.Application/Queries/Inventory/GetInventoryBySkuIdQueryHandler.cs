using SampleProject.Application.DTOs;
using SampleProject.Application.Mediator;
using SampleProject.Domain.Interfaces;

namespace SampleProject.Application.Queries.Inventory;

public class GetInventoryBySkuIdQueryHandler : IRequestHandler<GetInventoryBySkuIdQuery, List<InventoryDto>>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ISkuRepository _skuRepository;
    private readonly IStorageRepository _storageRepository;

    public GetInventoryBySkuIdQueryHandler(
        IInventoryRepository inventoryRepository,
        ISkuRepository skuRepository,
        IStorageRepository storageRepository)
    {
        _inventoryRepository = inventoryRepository;
        _skuRepository = skuRepository;
        _storageRepository = storageRepository;
    }

    public async Task<List<InventoryDto>> HandleAsync(
        GetInventoryBySkuIdQuery request,
        CancellationToken cancellationToken = default)
    {
        var sku = await _skuRepository.GetBySkuIdAsync(request.SkuId, cancellationToken);
        if (sku == null)
        {
            throw new KeyNotFoundException($"Sku with SkuId {request.SkuId} not found");
        }

        var inventories = await _inventoryRepository.GetBySkuIdAsync(sku.Id, cancellationToken);
        var result = new List<InventoryDto>();

        foreach (var inventory in inventories)
        {
            var storage = await _storageRepository.GetByIdAsync(inventory.StorageId, cancellationToken);
            if (storage != null)
            {
                result.Add(new InventoryDto(
                    sku.Id,
                    sku.SkuCode,
                    storage.Id,
                    storage.Name,
                    inventory.Quantity.Value, // 從 Quantity 提取原始值
                    inventory.UpdatedAt));
            }
        }

        return result;
    }
}
