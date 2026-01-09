using Microsoft.Extensions.Logging;
using SampleProject.Application.DTOs;
using SampleProject.Application.Mediator;
using SampleProject.Domain.Interfaces;

namespace SampleProject.Application.Commands.Inventory;

public class CreateOrUpdateInventoryCommandHandler : IRequestHandler<CreateOrUpdateInventoryCommand, InventoryDto>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ISkuRepository _skuRepository;
    private readonly IStorageRepository _storageRepository;
    private readonly ILogger<CreateOrUpdateInventoryCommandHandler> _logger;

    public CreateOrUpdateInventoryCommandHandler(
        IInventoryRepository inventoryRepository,
        ISkuRepository skuRepository,
        IStorageRepository storageRepository,
        ILogger<CreateOrUpdateInventoryCommandHandler> logger)
    {
        _inventoryRepository = inventoryRepository;
        _skuRepository = skuRepository;
        _storageRepository = storageRepository;
        _logger = logger;
    }

    public async Task<InventoryDto> HandleAsync(
        CreateOrUpdateInventoryCommand request,
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

        var existingInventory = await _inventoryRepository.GetBySkuIdAndStorageIdAsync(
            sku.Id,
            storage.Id,
            cancellationToken);

        if (existingInventory == null)
        {
            // 建立新庫存
            var inventory = new Domain.Entities.Inventory(sku.Id, storage.Id, request.Quantity);
            await _inventoryRepository.AddAsync(inventory, cancellationToken);
            await _inventoryRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Inventory created for SkuId: {SkuId}, StorageId: {StorageId}, Quantity: {Quantity}",
                request.SkuId, request.StorageId, request.Quantity);

            return new InventoryDto(
                sku.Id,
                sku.SkuCode,
                storage.Id,
                storage.Name,
                inventory.Quantity.Value, // 從 Quantity 提取原始值
                inventory.UpdatedAt);
        }
        else
        {
            // 更新現有庫存
            existingInventory.SetQuantity(request.Quantity);
            await _inventoryRepository.UpdateAsync(existingInventory, cancellationToken);
            await _inventoryRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Inventory updated for SkuId: {SkuId}, StorageId: {StorageId}, Quantity: {Quantity}",
                request.SkuId, request.StorageId, request.Quantity);

            return new InventoryDto(
                sku.Id,
                sku.SkuCode,
                storage.Id,
                storage.Name,
                existingInventory.Quantity,
                existingInventory.UpdatedAt);
        }
    }
}
