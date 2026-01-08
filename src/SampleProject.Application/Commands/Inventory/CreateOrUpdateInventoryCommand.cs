using SampleProject.Application.DTOs;
using SampleProject.Application.Mediator;

namespace SampleProject.Application.Commands.Inventory;

public record CreateOrUpdateInventoryCommand(
    Guid SkuId,
    Guid StorageId,
    int Quantity
) : IRequest<InventoryDto>;
