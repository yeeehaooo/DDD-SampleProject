using SampleProject.Application.DTOs;
using SampleProject.Application.Mediator;

namespace SampleProject.Application.Commands.Sku;

public record UpdateSkuCommand(
    Guid SkuId,
    string? SkuCode,
    decimal? Price,
    bool? IsActive
) : IRequest<SkuDto>;
