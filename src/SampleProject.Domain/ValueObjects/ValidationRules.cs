namespace SampleProject.Domain.ValueObjects;

/// <summary>
/// Value Objects 驗證規則常數
///
/// 用途：
/// 1. 集中管理驗證規則，避免重複定義
/// 2. 確保 FluentValidation 和 Value Objects 使用相同的規則
/// 3. 方便維護和修改
/// </summary>
public static class ValidationRules
{
    /// <summary>
    /// ProductName 驗證規則
    /// </summary>
    public static class ProductName
    {
        public const int MaxLength = 200;
    }

    /// <summary>
    /// Product Description 驗證規則
    /// </summary>
    public static class ProductDescription
    {
        public const int MaxLength = 1000;
    }

    /// <summary>
    /// Money 驗證規則
    /// </summary>
    public static class Money
    {
        public const decimal MinAmount = 0;
        public const string DefaultCurrency = "TWD";
    }

    /// <summary>
    /// Quantity 驗證規則
    /// </summary>
    public static class Quantity
    {
        public const int MinValue = 0;
    }

    /// <summary>
    /// Address 驗證規則
    /// </summary>
    public static class Address
    {
        public const int MaxStreetLength = 200;
        public const int MaxCityLength = 100;
        public const int MaxStateLength = 100;
        public const int MaxPostalCodeLength = 20;
        public const int MaxCountryLength = 100;
    }

    /// <summary>
    /// SkuCode 驗證規則
    /// </summary>
    public static class SkuCode
    {
        public const int MaxLength = 50;
    }

    /// <summary>
    /// Storage Name 驗證規則
    /// </summary>
    public static class StorageName
    {
        public const int MaxLength = 200;
    }
}
