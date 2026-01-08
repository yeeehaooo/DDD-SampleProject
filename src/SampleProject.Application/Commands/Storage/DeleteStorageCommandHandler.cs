using Microsoft.Extensions.Logging;
using SampleProject.Application.Mediator;
using SampleProject.Domain.Interfaces;

namespace SampleProject.Application.Commands.Storage;

public class DeleteStorageCommandHandler : IRequestHandler<DeleteStorageCommand>
{
    private readonly IStorageRepository _repository;
    private readonly ILogger<DeleteStorageCommandHandler> _logger;

    public DeleteStorageCommandHandler(
        IStorageRepository repository,
        ILogger<DeleteStorageCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Unit> HandleAsync(
        DeleteStorageCommand request,
        CancellationToken cancellationToken = default)
    {
        var storage = await _repository.GetByStorageIdAsync(request.StorageId, cancellationToken);
        if (storage == null)
        {
            throw new KeyNotFoundException($"Storage with StorageId {request.StorageId} not found");
        }

        await _repository.DeleteAsync(storage.Id, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Storage deleted with StorageId: {StorageId}", request.StorageId);

        return new Unit();
    }
}
