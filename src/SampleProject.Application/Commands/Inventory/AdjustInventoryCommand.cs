using SampleProject.Application.DTOs;
using SampleProject.Application.Mediator;

namespace SampleProject.Application.Commands.Inventory;

public record AdjustInventoryCommand(
    Guid SkuId,
    Guid StorageId,
    int AdjustmentQuantity
) : IRequest<InventoryDto>;
