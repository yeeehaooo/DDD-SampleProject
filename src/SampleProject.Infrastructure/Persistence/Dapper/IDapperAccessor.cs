using System.Data;
using Dapper;

namespace SampleProject.Infrastructure.Persistence.Dapper;

/// <summary>
/// Dapper 資料存取器介面
///
/// 提供常用方法，減少重複程式碼
/// 同時保留直接使用 Dapper 的彈性
/// </summary>
public interface IDapperAccessor
{
    /// <summary>
    /// 取得連線（用於複雜查詢，可直接使用 Dapper）
    /// </summary>
    IDbConnection GetConnection();

    /// <summary>
    /// 查詢單筆（可為 null）
    /// </summary>
    Task<T?> QueryFirstOrDefaultAsync<T>(
        string sql,
        object? parameters = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查詢多筆
    /// </summary>
    Task<List<T>> QueryAsync<T>(
        string sql,
        object? parameters = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查詢單筆（必須存在，否則拋出例外）
    /// </summary>
    Task<T> QuerySingleAsync<T>(
        string sql,
        object? parameters = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 執行命令（INSERT/UPDATE/DELETE）
    /// </summary>
    Task<int> ExecuteAsync(
        string sql,
        object? parameters = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 執行查詢並取得單一值
    /// </summary>
    Task<T> ExecuteScalarAsync<T>(
        string sql,
        object? parameters = null,
        CancellationToken cancellationToken = default);
}
