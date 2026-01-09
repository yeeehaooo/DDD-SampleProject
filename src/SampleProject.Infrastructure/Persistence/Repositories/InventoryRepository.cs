using Dapper;
using SampleProject.Domain.Entities;
using SampleProject.Domain.Interfaces;
using SampleProject.Infrastructure.Persistence.DbConnection;
using System.Data;

namespace SampleProject.Infrastructure.Persistence.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public InventoryRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Inventory?> GetBySkuIdAndStorageIdAsync(int skuId, int storageId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT SkuId, StorageId, Quantity, CreatedAt, UpdatedAt
            FROM Inventories
            WHERE SkuId = @SkuId AND StorageId = @StorageId";

        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QueryFirstOrDefaultAsync<Inventory>(
            new CommandDefinition(
                sql,
                new { SkuId = skuId, StorageId = storageId },
                cancellationToken: cancellationToken));

        return result;
    }

    public async Task<List<Inventory>> GetBySkuIdAsync(int skuId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT SkuId, StorageId, Quantity, CreatedAt, UpdatedAt
            FROM Inventories
            WHERE SkuId = @SkuId";

        using var connection = _connectionFactory.CreateConnection();
        var results = await connection.QueryAsync<Inventory>(
            new CommandDefinition(sql, new { SkuId = skuId }, cancellationToken: cancellationToken));

        return results.ToList();
    }

    public async Task<List<Inventory>> GetByStorageIdAsync(int storageId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT SkuId, StorageId, Quantity, CreatedAt, UpdatedAt
            FROM Inventories
            WHERE StorageId = @StorageId";

        using var connection = _connectionFactory.CreateConnection();
        var results = await connection.QueryAsync<Inventory>(
            new CommandDefinition(sql, new { StorageId = storageId }, cancellationToken: cancellationToken));

        return results.ToList();
    }

    public async Task AddAsync(Inventory inventory, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO Inventories (SkuId, StorageId, Quantity, CreatedAt, UpdatedAt)
            VALUES (@SkuId, @StorageId, @Quantity, @CreatedAt, @UpdatedAt)";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    inventory.SkuId,
                    inventory.StorageId,
                    Quantity = inventory.Quantity.Value, // 提取 Quantity 的原始值
                    inventory.CreatedAt,
                    inventory.UpdatedAt
                },
                cancellationToken: cancellationToken));
    }

    public async Task UpdateAsync(Inventory inventory, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            UPDATE Inventories
            SET Quantity = @Quantity,
                UpdatedAt = @UpdatedAt
            WHERE SkuId = @SkuId AND StorageId = @StorageId";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    inventory.SkuId,
                    inventory.StorageId,
                    inventory.Quantity,
                    inventory.UpdatedAt
                },
                cancellationToken: cancellationToken));
    }

    public async Task DeleteAsync(int skuId, int storageId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            DELETE FROM Inventories
            WHERE SkuId = @SkuId AND StorageId = @StorageId";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new { SkuId = skuId, StorageId = storageId },
                cancellationToken: cancellationToken));
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(0);
    }
}
