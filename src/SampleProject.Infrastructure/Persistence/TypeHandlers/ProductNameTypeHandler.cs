using Dapper;
using SampleProject.Domain.ValueObjects;
using System.Data;

namespace SampleProject.Infrastructure.Persistence.TypeHandlers;

/// <summary>
/// Dapper Type Handler for ProductName Value Object
/// 將資料庫的 string 映射到 ProductName，並將 ProductName 轉換回 string 寫入資料庫
/// </summary>
public class ProductNameTypeHandler : SqlMapper.TypeHandler<ProductName>
{
    public override void SetValue(IDbDataParameter parameter, ProductName? value)
    {
        parameter.Value = value?.Value ?? (object)DBNull.Value;
        parameter.DbType = DbType.String;
    }

    public override ProductName Parse(object value)
    {
        if (value == null || value == DBNull.Value)
            throw new InvalidOperationException("Cannot parse null or DBNull to ProductName");

        return new ProductName(value.ToString() ?? string.Empty);
    }
}
