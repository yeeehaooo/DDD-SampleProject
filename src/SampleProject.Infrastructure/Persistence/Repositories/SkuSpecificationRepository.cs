using Dapper;
using SampleProject.Domain.Entities;
using SampleProject.Domain.Interfaces;
using SampleProject.Infrastructure.Persistence.DbConnection;
using System.Data;

namespace SampleProject.Infrastructure.Persistence.Repositories;

public class SkuSpecificationRepository : ISkuSpecificationRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public SkuSpecificationRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<SkuSpecification>> GetBySkuIdAsync(int skuId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT SkuId, SpecificationValueId
            FROM SkuSpecifications
            WHERE SkuId = @SkuId";

        using var connection = _connectionFactory.CreateConnection();
        var results = await connection.QueryAsync<SkuSpecification>(
            new CommandDefinition(sql, new { SkuId = skuId }, cancellationToken: cancellationToken));

        return results.ToList();
    }

    public async Task AddAsync(SkuSpecification skuSpecification, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO SkuSpecifications (SkuId, SpecificationValueId)
            VALUES (@SkuId, @SpecificationValueId)";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    skuSpecification.SkuId,
                    skuSpecification.SpecificationValueId
                },
                cancellationToken: cancellationToken));
    }

    public async Task RemoveAsync(int skuId, int specificationValueId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            DELETE FROM SkuSpecifications
            WHERE SkuId = @SkuId AND SpecificationValueId = @SpecificationValueId";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new { SkuId = skuId, SpecificationValueId = specificationValueId },
                cancellationToken: cancellationToken));
    }

    public async Task RemoveAllBySkuIdAsync(int skuId, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM SkuSpecifications WHERE SkuId = @SkuId";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            new CommandDefinition(sql, new { SkuId = skuId }, cancellationToken: cancellationToken));
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(0);
    }
}
