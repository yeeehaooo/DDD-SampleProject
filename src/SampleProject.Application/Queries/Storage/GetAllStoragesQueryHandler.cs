using SampleProject.Application.DTOs;
using SampleProject.Application.Mediator;
using SampleProject.Domain.Interfaces;

namespace SampleProject.Application.Queries.Storage;

public class GetAllStoragesQueryHandler : IRequestHandler<GetAllStoragesQuery, List<StorageDto>>
{
    private readonly IStorageRepository _repository;

    public GetAllStoragesQueryHandler(IStorageRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<StorageDto>> HandleAsync(
        GetAllStoragesQuery request,
        CancellationToken cancellationToken = default)
    {
        var storages = await _repository.GetAllAsync(cancellationToken);
        return storages.Select(s => new StorageDto(
            s.Id,
            s.StorageId,
            s.Name,
            s.Address,
            s.IsActive,
            s.CreatedAt,
            s.UpdatedAt)).ToList();
    }
}
