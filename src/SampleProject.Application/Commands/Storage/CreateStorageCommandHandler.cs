using Microsoft.Extensions.Logging;
using SampleProject.Application.DTOs;
using SampleProject.Application.Mediator;
using SampleProject.Domain.Interfaces;

namespace SampleProject.Application.Commands.Storage;

public class CreateStorageCommandHandler : IRequestHandler<CreateStorageCommand, StorageDto>
{
    private readonly IStorageRepository _repository;
    private readonly ILogger<CreateStorageCommandHandler> _logger;

    public CreateStorageCommandHandler(
        IStorageRepository repository,
        ILogger<CreateStorageCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<StorageDto> HandleAsync(
        CreateStorageCommand request,
        CancellationToken cancellationToken = default)
    {
        var storage = new Domain.Entities.Storage(request.Name, request.Address);

        await _repository.AddAsync(storage, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Storage created with StorageId: {StorageId}", storage.StorageId);

        return new StorageDto(
            storage.Id,
            storage.StorageId,
            storage.Name,
            storage.Address,
            storage.IsActive,
            storage.CreatedAt,
            storage.UpdatedAt);
    }
}
