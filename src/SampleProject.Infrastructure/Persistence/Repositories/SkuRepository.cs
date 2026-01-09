using Dapper;
using SampleProject.Domain.Entities;
using SampleProject.Domain.Interfaces;
using SampleProject.Infrastructure.Persistence.DbConnection;
using System.Data;

namespace SampleProject.Infrastructure.Persistence.Repositories;

public class SkuRepository : ISkuRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public SkuRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Sku?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT Id, SkuId, ProductId, SkuCode, Price, IsActive, CreatedAt, UpdatedAt
            FROM Skus
            WHERE Id = @Id";

        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QueryFirstOrDefaultAsync<Sku>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));

        return result;
    }

    public async Task<Sku?> GetBySkuIdAsync(Guid skuId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT Id, SkuId, ProductId, SkuCode, Price, IsActive, CreatedAt, UpdatedAt
            FROM Skus
            WHERE SkuId = @SkuId";

        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QueryFirstOrDefaultAsync<Sku>(
            new CommandDefinition(sql, new { SkuId = skuId }, cancellationToken: cancellationToken));

        return result;
    }

    public async Task<List<Sku>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT Id, SkuId, ProductId, SkuCode, Price, IsActive, CreatedAt, UpdatedAt
            FROM Skus
            WHERE ProductId = @ProductId
            ORDER BY Id";

        using var connection = _connectionFactory.CreateConnection();
        var results = await connection.QueryAsync<Sku>(
            new CommandDefinition(sql, new { ProductId = productId }, cancellationToken: cancellationToken));

        return results.ToList();
    }

    public async Task<Sku?> GetBySkuCodeAsync(string skuCode, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT Id, SkuId, ProductId, SkuCode, Price, IsActive, CreatedAt, UpdatedAt
            FROM Skus
            WHERE SkuCode = @SkuCode";

        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QueryFirstOrDefaultAsync<Sku>(
            new CommandDefinition(sql, new { SkuCode = skuCode }, cancellationToken: cancellationToken));

        return result;
    }

    public async Task<List<Sku>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT Id, SkuId, ProductId, SkuCode, Price, IsActive, CreatedAt, UpdatedAt
            FROM Skus
            ORDER BY Id";

        using var connection = _connectionFactory.CreateConnection();
        var results = await connection.QueryAsync<Sku>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));

        return results.ToList();
    }

    public async Task AddAsync(Sku sku, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO Skus (SkuId, ProductId, SkuCode, Price, IsActive, CreatedAt, UpdatedAt)
            OUTPUT INSERTED.Id, INSERTED.SkuId, INSERTED.ProductId, INSERTED.SkuCode, INSERTED.Price, INSERTED.IsActive, INSERTED.CreatedAt, INSERTED.UpdatedAt
            VALUES (@SkuId, @ProductId, @SkuCode, @Price, @IsActive, @CreatedAt, @UpdatedAt)";

        using var connection = _connectionFactory.CreateConnection();
        var inserted = await connection.QuerySingleAsync<Sku>(
            new CommandDefinition(
                sql,
                new
                {
                    sku.SkuId,
                    sku.ProductId,
                    sku.SkuCode,
                    Price = sku.Price?.Amount, // 提取 Money 的原始值（可空）
                    sku.IsActive,
                    sku.CreatedAt,
                    sku.UpdatedAt
                },
                cancellationToken: cancellationToken));

        // 更新 sku 的 Id（從資料庫返回的值）
        var idProperty = typeof(Sku).GetProperty("Id", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        if (idProperty != null)
        {
            var setMethod = idProperty.GetSetMethod(true);
            if (setMethod != null)
            {
                setMethod.Invoke(sku, new object[] { inserted.Id });
            }
        }
    }

    public async Task UpdateAsync(Sku sku, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            UPDATE Skus
            SET SkuCode = @SkuCode,
                Price = @Price,
                IsActive = @IsActive,
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    sku.Id,
                    sku.SkuCode,
                    Price = sku.Price?.Amount, // 提取 Money 的原始值（可空）
                    sku.IsActive,
                    sku.UpdatedAt
                },
                cancellationToken: cancellationToken));
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM Skus WHERE Id = @Id";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(0);
    }
}
