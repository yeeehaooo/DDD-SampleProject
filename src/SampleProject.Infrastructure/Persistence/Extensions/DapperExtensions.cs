using Dapper;
using SampleProject.Domain.ValueObjects;
using SampleProject.Infrastructure.Persistence.TypeHandlers;

namespace SampleProject.Infrastructure.Persistence.Extensions;

/// <summary>
/// Dapper 擴展方法 - 註冊 Value Objects 的 Type Handlers
/// </summary>
public static class DapperExtensions
{
    /// <summary>
    /// 註冊所有 Value Objects 的 Type Handlers
    /// 應在應用程式啟動時呼叫一次
    /// </summary>
    public static void RegisterValueObjectTypeHandlers()
    {
        SqlMapper.AddTypeHandler(new MoneyTypeHandler());
        SqlMapper.AddTypeHandler(new ProductNameTypeHandler());
        SqlMapper.AddTypeHandler(new AddressTypeHandler());
        SqlMapper.AddTypeHandler(new QuantityTypeHandler());

        // 註冊可空類型的 Handlers
        // 注意：不能使用 typeof(Money?)，直接傳入 handler 實例，Dapper 會從泛型參數推斷型別
        SqlMapper.AddTypeHandler(new NullableMoneyTypeHandler());
        SqlMapper.AddTypeHandler(new NullableAddressTypeHandler());
    }
}

/// <summary>
/// 可空的 Money Type Handler
/// </summary>
public class NullableMoneyTypeHandler : SqlMapper.TypeHandler<Money?>
{
    private readonly MoneyTypeHandler _handler = new();

    public override void SetValue(System.Data.IDbDataParameter parameter, Money? value)
    {
        _handler.SetValue(parameter, value);
    }

    public override Money? Parse(object value)
    {
        if (value == null || value == DBNull.Value)
            return null;

        return _handler.Parse(value);
    }
}

/// <summary>
/// 可空的 Address Type Handler
/// </summary>
public class NullableAddressTypeHandler : SqlMapper.TypeHandler<Address?>
{
    private readonly AddressTypeHandler _handler = new();

    public override void SetValue(System.Data.IDbDataParameter parameter, Address? value)
    {
        _handler.SetValue(parameter, value);
    }

    public override Address? Parse(object value)
    {
        if (value == null || value == DBNull.Value)
            return null;

        return _handler.Parse(value);
    }
}
