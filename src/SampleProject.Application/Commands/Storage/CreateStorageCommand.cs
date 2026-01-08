using SampleProject.Application.DTOs;
using SampleProject.Application.Mediator;

namespace SampleProject.Application.Commands.Storage;

public record CreateStorageCommand(
    string Name,
    string? Address
) : IRequest<StorageDto>;
