using Microsoft.Extensions.Logging;
using SampleProject.Application.DTOs;
using SampleProject.Application.Mediator;
using SampleProject.Domain.Interfaces;

namespace SampleProject.Application.Commands.Storage;

public class UpdateStorageCommandHandler : IRequestHandler<UpdateStorageCommand, StorageDto>
{
    private readonly IStorageRepository _repository;
    private readonly ILogger<UpdateStorageCommandHandler> _logger;

    public UpdateStorageCommandHandler(
        IStorageRepository repository,
        ILogger<UpdateStorageCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<StorageDto> HandleAsync(
        UpdateStorageCommand request,
        CancellationToken cancellationToken = default)
    {
        var storage = await _repository.GetByStorageIdAsync(request.StorageId, cancellationToken);
        if (storage == null)
        {
            throw new KeyNotFoundException($"Storage with StorageId {request.StorageId} not found");
        }

        if (request.Name != null)
        {
            storage.UpdateName(request.Name);
        }

        if (request.Address != null)
        {
            storage.UpdateAddress(request.Address);
        }

        if (request.IsActive.HasValue)
        {
            if (request.IsActive.Value)
                storage.Activate();
            else
                storage.Deactivate();
        }

        await _repository.UpdateAsync(storage, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Storage updated with StorageId: {StorageId}", storage.StorageId);

        return new StorageDto(
            storage.Id,
            storage.StorageId,
            storage.Name,
            storage.Address?.ToString(), // 從 Address 提取字串表示
            storage.IsActive,
            storage.CreatedAt,
            storage.UpdatedAt);
    }
}
