using SampleProject.Application.DTOs;
using SampleProject.Application.Mediator;

namespace SampleProject.Application.Commands.Storage;

public record UpdateStorageCommand(
    Guid StorageId,
    string? Name,
    string? Address,
    bool? IsActive
) : IRequest<StorageDto>;
