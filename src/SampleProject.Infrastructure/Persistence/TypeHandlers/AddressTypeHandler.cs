using Dapper;
using SampleProject.Domain.ValueObjects;
using System.Data;

namespace SampleProject.Infrastructure.Persistence.TypeHandlers;

/// <summary>
/// Dapper Type Handler for Address Value Object
/// 將資料庫的 string 映射到 Address，並將 Address 轉換回 string 寫入資料庫
/// 注意：此實作將 Address 序列化為字串（使用 ToString），簡化版本
/// </summary>
public class AddressTypeHandler : SqlMapper.TypeHandler<Address>
{
    public override void SetValue(IDbDataParameter parameter, Address? value)
    {
        if (value == null || value.IsEmpty)
        {
            parameter.Value = DBNull.Value;
        }
        else
        {
            // 簡化版本：將 Address 轉為字串（僅使用 Street）
            // 如果需要完整序列化，可以使用 JSON
            parameter.Value = value.Street;
        }
        parameter.DbType = DbType.String;
    }

    public override Address Parse(object value)
    {
        if (value == null || value == DBNull.Value)
            return null!;

        var addressString = value.ToString();
        if (string.IsNullOrWhiteSpace(addressString))
            return null!;

        // 簡化版本：從字串建立 Address（僅使用 Street）
        // 如果需要完整反序列化，可以使用 JSON
        return new Address(addressString);
    }
}
