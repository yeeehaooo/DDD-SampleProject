using SampleProject.Application.Mediator;

namespace SampleProject.Application.Commands.Sku;

public record DeleteSkuCommand(Guid SkuId) : IRequest;
