using Dapper;
using SampleProject.Domain.Entities;
using SampleProject.Domain.Interfaces;
using SampleProject.Infrastructure.Persistence.DbConnection;
using System.Data;

namespace SampleProject.Infrastructure.Persistence.Repositories;

public class SpecificationRepository : ISpecificationRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public SpecificationRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Specification?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT Id, SpecificationId, Name, DisplayOrder, CreatedAt, UpdatedAt
            FROM Specifications
            WHERE Id = @Id";

        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QueryFirstOrDefaultAsync<Specification>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));

        return result;
    }

    public async Task<Specification?> GetBySpecificationIdAsync(Guid specificationId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT Id, SpecificationId, Name, DisplayOrder, CreatedAt, UpdatedAt
            FROM Specifications
            WHERE SpecificationId = @SpecificationId";

        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QueryFirstOrDefaultAsync<Specification>(
            new CommandDefinition(sql, new { SpecificationId = specificationId }, cancellationToken: cancellationToken));

        return result;
    }

    public async Task<List<Specification>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT Id, SpecificationId, Name, DisplayOrder, CreatedAt, UpdatedAt
            FROM Specifications
            ORDER BY DisplayOrder, Id";

        using var connection = _connectionFactory.CreateConnection();
        var results = await connection.QueryAsync<Specification>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));

        return results.ToList();
    }

    public async Task AddAsync(Specification specification, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO Specifications (SpecificationId, Name, DisplayOrder, CreatedAt, UpdatedAt)
            OUTPUT INSERTED.Id, INSERTED.SpecificationId, INSERTED.Name, INSERTED.DisplayOrder, INSERTED.CreatedAt, INSERTED.UpdatedAt
            VALUES (@SpecificationId, @Name, @DisplayOrder, @CreatedAt, @UpdatedAt)";

        using var connection = _connectionFactory.CreateConnection();
        var inserted = await connection.QuerySingleAsync<Specification>(
            new CommandDefinition(
                sql,
                new
                {
                    specification.SpecificationId,
                    specification.Name,
                    specification.DisplayOrder,
                    specification.CreatedAt,
                    specification.UpdatedAt
                },
                cancellationToken: cancellationToken));

        // 更新 specification 的 Id（從資料庫返回的值）
        var idProperty = typeof(Specification).GetProperty("Id", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        if (idProperty != null)
        {
            var setMethod = idProperty.GetSetMethod(true);
            if (setMethod != null)
            {
                setMethod.Invoke(specification, new object[] { inserted.Id });
            }
        }
    }

    public async Task UpdateAsync(Specification specification, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            UPDATE Specifications
            SET Name = @Name,
                DisplayOrder = @DisplayOrder,
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    specification.Id,
                    specification.Name,
                    specification.DisplayOrder,
                    specification.UpdatedAt
                },
                cancellationToken: cancellationToken));
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(0);
    }
}
