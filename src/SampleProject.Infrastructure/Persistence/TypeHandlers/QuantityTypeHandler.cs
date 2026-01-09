using Dapper;
using SampleProject.Domain.ValueObjects;
using System.Data;

namespace SampleProject.Infrastructure.Persistence.TypeHandlers;

/// <summary>
/// Dapper Type Handler for Quantity Value Object
/// 將資料庫的 int 映射到 Quantity，並將 Quantity 轉換回 int 寫入資料庫
/// </summary>
public class QuantityTypeHandler : SqlMapper.TypeHandler<Quantity>
{
    public override void SetValue(IDbDataParameter parameter, Quantity? value)
    {
        parameter.Value = value?.Value ?? (object)DBNull.Value;
        parameter.DbType = DbType.Int32;
    }

    public override Quantity Parse(object value)
    {
        if (value == null || value == DBNull.Value)
            throw new InvalidOperationException("Cannot parse null or DBNull to Quantity");

        if (value is int intValue)
        {
            return new Quantity(intValue);
        }

        if (int.TryParse(value.ToString(), out var parsed))
        {
            return new Quantity(parsed);
        }

        throw new InvalidOperationException($"Cannot parse {value.GetType()} to Quantity");
    }
}
