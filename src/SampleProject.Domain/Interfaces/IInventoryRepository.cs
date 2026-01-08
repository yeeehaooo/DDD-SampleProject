using SampleProject.Domain.Entities;

namespace SampleProject.Domain.Interfaces;

public interface IInventoryRepository
{
    Task<Inventory?> GetBySkuIdAndStorageIdAsync(int skuId, int storageId, CancellationToken cancellationToken = default);
    Task<List<Inventory>> GetBySkuIdAsync(int skuId, CancellationToken cancellationToken = default);
    Task<List<Inventory>> GetByStorageIdAsync(int storageId, CancellationToken cancellationToken = default);
    Task AddAsync(Inventory inventory, CancellationToken cancellationToken = default);
    Task UpdateAsync(Inventory inventory, CancellationToken cancellationToken = default);
    Task DeleteAsync(int skuId, int storageId, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
