using Dapper;
using SampleProject.Domain.Entities;
using SampleProject.Domain.Interfaces;
using SampleProject.Infrastructure.Persistence.DbConnection;
using System.Data;

namespace SampleProject.Infrastructure.Persistence.Repositories;

public class StorageRepository : IStorageRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public StorageRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Storage?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT Id, StorageId, Name, Address, IsActive, CreatedAt, UpdatedAt
            FROM Storages
            WHERE Id = @Id";

        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QueryFirstOrDefaultAsync<Storage>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));

        return result;
    }

    public async Task<Storage?> GetByStorageIdAsync(Guid storageId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT Id, StorageId, Name, Address, IsActive, CreatedAt, UpdatedAt
            FROM Storages
            WHERE StorageId = @StorageId";

        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QueryFirstOrDefaultAsync<Storage>(
            new CommandDefinition(sql, new { StorageId = storageId }, cancellationToken: cancellationToken));

        return result;
    }

    public async Task<List<Storage>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT Id, StorageId, Name, Address, IsActive, CreatedAt, UpdatedAt
            FROM Storages
            ORDER BY Id";

        using var connection = _connectionFactory.CreateConnection();
        var results = await connection.QueryAsync<Storage>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));

        return results.ToList();
    }

    public async Task<List<Storage>> GetActiveStoragesAsync(CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT Id, StorageId, Name, Address, IsActive, CreatedAt, UpdatedAt
            FROM Storages
            WHERE IsActive = 1
            ORDER BY Id";

        using var connection = _connectionFactory.CreateConnection();
        var results = await connection.QueryAsync<Storage>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));

        return results.ToList();
    }

    public async Task AddAsync(Storage storage, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO Storages (StorageId, Name, Address, IsActive, CreatedAt, UpdatedAt)
            OUTPUT INSERTED.Id, INSERTED.StorageId, INSERTED.Name, INSERTED.Address, INSERTED.IsActive, INSERTED.CreatedAt, INSERTED.UpdatedAt
            VALUES (@StorageId, @Name, @Address, @IsActive, @CreatedAt, @UpdatedAt)";

        using var connection = _connectionFactory.CreateConnection();
        var inserted = await connection.QuerySingleAsync<Storage>(
            new CommandDefinition(
                sql,
                new
                {
                    storage.StorageId,
                    storage.Name,
                    storage.Address,
                    storage.IsActive,
                    storage.CreatedAt,
                    storage.UpdatedAt
                },
                cancellationToken: cancellationToken));

        // 更新 storage 的 Id（從資料庫返回的值）
        var idProperty = typeof(Storage).GetProperty("Id", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        if (idProperty != null)
        {
            var setMethod = idProperty.GetSetMethod(true);
            if (setMethod != null)
            {
                setMethod.Invoke(storage, new object[] { inserted.Id });
            }
        }
    }

    public async Task UpdateAsync(Storage storage, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            UPDATE Storages
            SET Name = @Name,
                Address = @Address,
                IsActive = @IsActive,
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    storage.Id,
                    storage.Name,
                    storage.Address,
                    storage.IsActive,
                    storage.UpdatedAt
                },
                cancellationToken: cancellationToken));
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM Storages WHERE Id = @Id";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(0);
    }
}
