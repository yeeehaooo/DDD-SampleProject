using System.Data;
using Dapper;
using Microsoft.Extensions.Logging;
using SampleProject.Infrastructure.Persistence.DbConnection;

namespace SampleProject.Infrastructure.Persistence.Dapper;

/// <summary>
/// Dapper 資料存取器實作
/// </summary>
public class DapperAccessor : IDapperAccessor
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<DapperAccessor>? _logger;

    public DapperAccessor(
        IDbConnectionFactory connectionFactory,
        ILogger<DapperAccessor>? logger = null)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public IDbConnection GetConnection()
    {
        return _connectionFactory.CreateConnection();
    }

    public async Task<T?> QueryFirstOrDefaultAsync<T>(
        string sql,
        object? parameters = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            var result = await connection.QueryFirstOrDefaultAsync<T>(
                new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

            _logger?.LogDebug(
                "QueryFirstOrDefaultAsync<{TypeName}> executed in {ElapsedMs}ms",
                typeof(T).Name,
                stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "QueryFirstOrDefaultAsync<{TypeName}> failed", typeof(T).Name);
            throw;
        }
    }

    public async Task<List<T>> QueryAsync<T>(
        string sql,
        object? parameters = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            var results = await connection.QueryAsync<T>(
                new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

            var list = results.ToList();

            _logger?.LogDebug(
                "QueryAsync<{TypeName}> returned {Count} records in {ElapsedMs}ms",
                typeof(T).Name,
                list.Count,
                stopwatch.ElapsedMilliseconds);

            return list;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "QueryAsync<{TypeName}> failed", typeof(T).Name);
            throw;
        }
    }

    public async Task<T> QuerySingleAsync<T>(
        string sql,
        object? parameters = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            var result = await connection.QuerySingleAsync<T>(
                new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

            _logger?.LogDebug(
                "QuerySingleAsync<{TypeName}> executed in {ElapsedMs}ms",
                typeof(T).Name,
                stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "QuerySingleAsync<{TypeName}> failed", typeof(T).Name);
            throw;
        }
    }

    public async Task<int> ExecuteAsync(
        string sql,
        object? parameters = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            var rowsAffected = await connection.ExecuteAsync(
                new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

            _logger?.LogDebug(
                "ExecuteAsync affected {RowsAffected} rows in {ElapsedMs}ms",
                rowsAffected,
                stopwatch.ElapsedMilliseconds);

            return rowsAffected;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "ExecuteAsync failed");
            throw;
        }
    }

    public async Task<T> ExecuteScalarAsync<T>(
        string sql,
        object? parameters = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            var result = await connection.ExecuteScalarAsync<T>(
                new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

            _logger?.LogDebug(
                "ExecuteScalarAsync<{TypeName}> executed in {ElapsedMs}ms",
                typeof(T).Name,
                stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "ExecuteScalarAsync<{TypeName}> failed", typeof(T).Name);
            throw;
        }
    }
}
