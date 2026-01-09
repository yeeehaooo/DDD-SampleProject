using Dapper;
using SampleProject.Domain.ValueObjects;
using System.Data;

namespace SampleProject.Infrastructure.Persistence.TypeHandlers;

/// <summary>
/// Dapper Type Handler for Money Value Object
/// 將資料庫的 decimal 映射到 Money，並將 Money 轉換回 decimal 寫入資料庫
/// </summary>
public class MoneyTypeHandler : SqlMapper.TypeHandler<Money>
{
    public override void SetValue(IDbDataParameter parameter, Money? value)
    {
        parameter.Value = value?.Amount ?? (object)DBNull.Value;
        parameter.DbType = DbType.Decimal;
    }

    public override Money Parse(object value)
    {
        if (value == null || value == DBNull.Value)
            throw new InvalidOperationException("Cannot parse null or DBNull to Money");

        if (value is decimal decimalValue)
        {
            return new Money(decimalValue, "TWD");
        }

        if (decimal.TryParse(value.ToString(), out var parsed))
        {
            return new Money(parsed, "TWD");
        }

        throw new InvalidOperationException($"Cannot parse {value.GetType()} to Money");
    }
}
