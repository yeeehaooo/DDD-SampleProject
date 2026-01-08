using Dapper;
using SampleProject.Domain.Entities;
using SampleProject.Domain.Interfaces;
using SampleProject.Infrastructure.Persistence.DbConnection;
using System.Data;

namespace SampleProject.Infrastructure.Persistence.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ProductRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT Id, ProductId, Name, Description, BasePrice, CreatedAt, UpdatedAt
            FROM Products
            WHERE Id = @Id";

        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QueryFirstOrDefaultAsync<Product>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));

        return result;
    }

    public async Task<Product?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT Id, ProductId, Name, Description, BasePrice, CreatedAt, UpdatedAt
            FROM Products
            WHERE ProductId = @ProductId";

        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QueryFirstOrDefaultAsync<Product>(
            new CommandDefinition(sql, new { ProductId = productId }, cancellationToken: cancellationToken));

        return result;
    }

    public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT Id, ProductId, Name, Description, BasePrice, CreatedAt, UpdatedAt
            FROM Products
            ORDER BY Id";

        using var connection = _connectionFactory.CreateConnection();
        var results = await connection.QueryAsync<Product>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));

        // Dapper 的 QueryAsync 已經將結果載入記憶體，這裡轉為 List 確保具體化
        // 避免返回延遲執行的迭代器，提升序列化效能
        return results.ToList();
    }

    public async Task<IEnumerable<Product>> GetByPageAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT Id, ProductId, Name, Description, BasePrice, CreatedAt, UpdatedAt
            FROM Products
            ORDER BY Id
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY";

        var offset = (pageNumber - 1) * pageSize;

        using var connection = _connectionFactory.CreateConnection();
        var results = await connection.QueryAsync<Product>(
            new CommandDefinition(
                sql,
                new { Offset = offset, PageSize = pageSize },
                cancellationToken: cancellationToken));

        // 分頁查詢結果通常較小，轉為 List 確保具體化
        return results.ToList();
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO Products (ProductId, Name, Description, BasePrice, CreatedAt, UpdatedAt)
            OUTPUT INSERTED.Id, INSERTED.ProductId, INSERTED.Name, INSERTED.Description, INSERTED.BasePrice, INSERTED.CreatedAt, INSERTED.UpdatedAt
            VALUES (@ProductId, @Name, @Description, @BasePrice, @CreatedAt, @UpdatedAt)";

        using var connection = _connectionFactory.CreateConnection();
        var inserted = await connection.QuerySingleAsync<Product>(
            new CommandDefinition(
                sql,
                new
                {
                    product.ProductId,
                    product.Name,
                    product.Description,
                    product.BasePrice,
                    product.CreatedAt,
                    product.UpdatedAt
                },
                cancellationToken: cancellationToken));

        // 更新 product 的 Id（從資料庫返回的值）
        var idProperty = typeof(Product).GetProperty("Id", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        if (idProperty != null)
        {
            var setMethod = idProperty.GetSetMethod(true); // 取得 private setter
            if (setMethod != null)
            {
                setMethod.Invoke(product, new object[] { inserted.Id });
            }
        }
    }

    public async Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            UPDATE Products
            SET Name = @Name,
                Description = @Description,
                BasePrice = @BasePrice,
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    product.Id,
                    product.Name,
                    product.Description,
                    product.BasePrice,
                    product.UpdatedAt
                },
                cancellationToken: cancellationToken));
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM Products WHERE Id = @Id";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Dapper 不需要 SaveChanges，每個操作都是立即執行
        // 此方法保留是為了符合介面，但實際上不需要做任何事
        return Task.FromResult(0);
    }
}
