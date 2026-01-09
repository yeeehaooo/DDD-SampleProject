using SampleProject.Application.DTOs;
using SampleProject.Application.Mediator;
using SampleProject.Domain.Interfaces;

namespace SampleProject.Application.Queries.Storage;

public class GetActiveStoragesQueryHandler : IRequestHandler<GetActiveStoragesQuery, List<StorageDto>>
{
    private readonly IStorageRepository _repository;

    public GetActiveStoragesQueryHandler(IStorageRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<StorageDto>> HandleAsync(
        GetActiveStoragesQuery request,
        CancellationToken cancellationToken = default)
    {
        var storages = await _repository.GetActiveStoragesAsync(cancellationToken);
        return storages.Select(s => new StorageDto(
            s.Id,
            s.StorageId,
            s.Name,
            s.Address?.ToString(), // 從 Address 提取字串表示
            s.IsActive,
            s.CreatedAt,
            s.UpdatedAt)).ToList();
    }
}
