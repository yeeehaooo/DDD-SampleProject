using SampleProject.Domain.Entities;

namespace SampleProject.Domain.Interfaces;

public interface IStorageRepository
{
    Task<Storage?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Storage?> GetByStorageIdAsync(Guid storageId, CancellationToken cancellationToken = default);
    Task<List<Storage>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<Storage>> GetActiveStoragesAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Storage storage, CancellationToken cancellationToken = default);
    Task UpdateAsync(Storage storage, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
