using SampleProject.Application.DTOs;
using SampleProject.Application.Mediator;

namespace SampleProject.Application.Queries.Inventory;

public record GetInventoryByStorageIdQuery(Guid StorageId) : IRequest<List<InventoryDto>>;
