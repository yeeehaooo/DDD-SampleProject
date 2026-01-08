using SampleProject.Domain.Entities;

namespace SampleProject.Domain.Interfaces;

public interface ISkuSpecificationRepository
{
    Task<List<SkuSpecification>> GetBySkuIdAsync(int skuId, CancellationToken cancellationToken = default);
    Task AddAsync(SkuSpecification skuSpecification, CancellationToken cancellationToken = default);
    Task RemoveAsync(int skuId, int specificationValueId, CancellationToken cancellationToken = default);
    Task RemoveAllBySkuIdAsync(int skuId, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
