using Microsoft.Extensions.Logging;
using SampleProject.Application.DTOs;
using SampleProject.Application.Mediator;
using SampleProject.Domain.Interfaces;

namespace SampleProject.Application.Commands.Inventory;

public class AdjustInventoryCommandHandler : IRequestHandler<AdjustInventoryCommand, InventoryDto>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ISkuRepository _skuRepository;
    private readonly IStorageRepository _storageRepository;
    private readonly ILogger<AdjustInventoryCommandHandler> _logger;

    public AdjustInventoryCommandHandler(
        IInventoryRepository inventoryRepository,
        ISkuRepository skuRepository,
        IStorageRepository storageRepository,
        ILogger<AdjustInventoryCommandHandler> logger)
    {
        _inventoryRepository = inventoryRepository;
        _skuRepository = skuRepository;
        _storageRepository = storageRepository;
        _logger = logger;
    }

    public async Task<InventoryDto> HandleAsync(
        AdjustInventoryCommand request,
        CancellationToken cancellationToken = default)
    {
        var sku = await _skuRepository.GetBySkuIdAsync(request.SkuId, cancellationToken);
        if (sku == null)
        {
            throw new KeyNotFoundException($"Sku with SkuId {request.SkuId} not found");
        }

        var storage = await _storageRepository.GetByStorageIdAsync(request.StorageId, cancellationToken);
        if (storage == null)
        {
            throw new KeyNotFoundException($"Storage with StorageId {request.StorageId} not found");
        }

        var inventory = await _inventoryRepository.GetBySkuIdAndStorageIdAsync(
            sku.Id,
            storage.Id,
            cancellationToken);

        if (inventory == null)
        {
            // 如果庫存不存在，建立新庫存
            inventory = new Domain.Entities.Inventory(sku.Id, storage.Id, 0);
            await _inventoryRepository.AddAsync(inventory, cancellationToken);
            await _inventoryRepository.SaveChangesAsync(cancellationToken);
        }

        // 調整庫存
        if (request.AdjustmentQuantity > 0)
        {
            inventory.IncreaseQuantity(request.AdjustmentQuantity);
        }
        else if (request.AdjustmentQuantity < 0)
        {
            inventory.DecreaseQuantity(Math.Abs(request.AdjustmentQuantity));
        }

        await _inventoryRepository.UpdateAsync(inventory, cancellationToken);
        await _inventoryRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Inventory adjusted for SkuId: {SkuId}, StorageId: {StorageId}, Adjustment: {Adjustment}, New Quantity: {Quantity}",
            request.SkuId, request.StorageId, request.AdjustmentQuantity, inventory.Quantity.Value);

        return new InventoryDto(
            sku.Id,
            sku.SkuCode,
            storage.Id,
            storage.Name,
            inventory.Quantity.Value, // 從 Quantity 提取原始值
            inventory.UpdatedAt);
    }
}
