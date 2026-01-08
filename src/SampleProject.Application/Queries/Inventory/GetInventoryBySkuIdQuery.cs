using SampleProject.Application.DTOs;
using SampleProject.Application.Mediator;

namespace SampleProject.Application.Queries.Inventory;

public record GetInventoryBySkuIdQuery(Guid SkuId) : IRequest<List<InventoryDto>>;
