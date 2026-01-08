using SampleProject.Application.DTOs;
using SampleProject.Application.Mediator;

namespace SampleProject.Application.Commands.Sku;

public record CreateSkuCommand(
    int ProductId,
    string SkuCode,
    decimal? Price,
    List<int> SpecificationValueIds
) : IRequest<SkuDto>;
