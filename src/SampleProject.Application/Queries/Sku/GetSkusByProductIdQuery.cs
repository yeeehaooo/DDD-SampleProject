using SampleProject.Application.DTOs;
using SampleProject.Application.Mediator;

namespace SampleProject.Application.Queries.Sku;

public record GetSkusByProductIdQuery(int ProductId) : IRequest<List<SkuDto>>;
