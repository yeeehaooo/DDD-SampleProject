using SampleProject.Application.Mediator;

namespace SampleProject.Application.Commands.Storage;

public record DeleteStorageCommand(Guid StorageId) : IRequest;
