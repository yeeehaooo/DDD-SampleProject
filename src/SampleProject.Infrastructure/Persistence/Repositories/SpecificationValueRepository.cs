using Dapper;
using SampleProject.Domain.Entities;
using SampleProject.Domain.Interfaces;
using SampleProject.Infrastructure.Persistence.DbConnection;
using System.Data;

namespace SampleProject.Infrastructure.Persistence.Repositories;

public class SpecificationValueRepository : ISpecificationValueRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public SpecificationValueRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<SpecificationValue?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT Id, SpecificationValueId, SpecificationId, Value, DisplayOrder, CreatedAt, UpdatedAt
            FROM SpecificationValues
            WHERE Id = @Id";

        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QueryFirstOrDefaultAsync<SpecificationValue>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));

        return result;
    }

    public async Task<List<SpecificationValue>> GetBySpecificationIdAsync(int specificationId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT Id, SpecificationValueId, SpecificationId, Value, DisplayOrder, CreatedAt, UpdatedAt
            FROM SpecificationValues
            WHERE SpecificationId = @SpecificationId
            ORDER BY DisplayOrder, Id";

        using var connection = _connectionFactory.CreateConnection();
        var results = await connection.QueryAsync<SpecificationValue>(
            new CommandDefinition(sql, new { SpecificationId = specificationId }, cancellationToken: cancellationToken));

        return results.ToList();
    }

    public async Task AddAsync(SpecificationValue specificationValue, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO SpecificationValues (SpecificationValueId, SpecificationId, Value, DisplayOrder, CreatedAt, UpdatedAt)
            OUTPUT INSERTED.Id, INSERTED.SpecificationValueId, INSERTED.SpecificationId, INSERTED.Value, INSERTED.DisplayOrder, INSERTED.CreatedAt, INSERTED.UpdatedAt
            VALUES (@SpecificationValueId, @SpecificationId, @Value, @DisplayOrder, @CreatedAt, @UpdatedAt)";

        using var connection = _connectionFactory.CreateConnection();
        var inserted = await connection.QuerySingleAsync<SpecificationValue>(
            new CommandDefinition(
                sql,
                new
                {
                    specificationValue.SpecificationValueId,
                    specificationValue.SpecificationId,
                    specificationValue.Value,
                    specificationValue.DisplayOrder,
                    specificationValue.CreatedAt,
                    specificationValue.UpdatedAt
                },
                cancellationToken: cancellationToken));

        // 更新 specificationValue 的 Id（從資料庫返回的值）
        var idProperty = typeof(SpecificationValue).GetProperty("Id", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        if (idProperty != null)
        {
            var setMethod = idProperty.GetSetMethod(true);
            if (setMethod != null)
            {
                setMethod.Invoke(specificationValue, new object[] { inserted.Id });
            }
        }
    }

    public async Task UpdateAsync(SpecificationValue specificationValue, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            UPDATE SpecificationValues
            SET Value = @Value,
                DisplayOrder = @DisplayOrder,
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    specificationValue.Id,
                    specificationValue.Value,
                    specificationValue.DisplayOrder,
                    specificationValue.UpdatedAt
                },
                cancellationToken: cancellationToken));
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(0);
    }
}
