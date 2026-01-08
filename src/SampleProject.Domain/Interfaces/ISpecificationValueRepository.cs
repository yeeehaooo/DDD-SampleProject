using SampleProject.Domain.Entities;

namespace SampleProject.Domain.Interfaces;

public interface ISpecificationValueRepository
{
    Task<SpecificationValue?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<SpecificationValue>> GetBySpecificationIdAsync(int specificationId, CancellationToken cancellationToken = default);
    Task AddAsync(SpecificationValue specificationValue, CancellationToken cancellationToken = default);
    Task UpdateAsync(SpecificationValue specificationValue, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
