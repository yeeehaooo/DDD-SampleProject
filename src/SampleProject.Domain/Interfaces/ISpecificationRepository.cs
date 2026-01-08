using SampleProject.Domain.Entities;

namespace SampleProject.Domain.Interfaces;

public interface ISpecificationRepository
{
    Task<Specification?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Specification?> GetBySpecificationIdAsync(Guid specificationId, CancellationToken cancellationToken = default);
    Task<List<Specification>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Specification specification, CancellationToken cancellationToken = default);
    Task UpdateAsync(Specification specification, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
