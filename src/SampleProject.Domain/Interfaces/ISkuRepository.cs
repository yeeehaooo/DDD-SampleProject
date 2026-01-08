using SampleProject.Domain.Entities;

namespace SampleProject.Domain.Interfaces;

public interface ISkuRepository
{
    Task<Sku?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Sku?> GetBySkuIdAsync(Guid skuId, CancellationToken cancellationToken = default);
    Task<List<Sku>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default);
    Task<Sku?> GetBySkuCodeAsync(string skuCode, CancellationToken cancellationToken = default);
    Task<List<Sku>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Sku sku, CancellationToken cancellationToken = default);
    Task UpdateAsync(Sku sku, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
