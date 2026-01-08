using SampleProject.Application.DTOs;
using SampleProject.Application.Mediator;

namespace SampleProject.Application.Queries.Sku;

public record GetSkuByIdQuery(Guid SkuId) : IRequest<SkuDto?>;
