using SampleProject.Application.DTOs;
using SampleProject.Application.Mediator;
using SampleProject.Domain.Interfaces;

namespace SampleProject.Application.Queries.Storage;

public class GetStorageByIdQueryHandler : IRequestHandler<GetStorageByIdQuery, StorageDto?>
{
    private readonly IStorageRepository _repository;

    public GetStorageByIdQueryHandler(IStorageRepository repository)
    {
        _repository = repository;
    }

    public async Task<StorageDto?> HandleAsync(
        GetStorageByIdQuery request,
        CancellationToken cancellationToken = default)
    {
        var storage = await _repository.GetByStorageIdAsync(request.StorageId, cancellationToken);
        if (storage == null)
            return null;

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
