using SampleProject.Domain.Exceptions;

namespace SampleProject.Domain.ValueObjects;

/// <summary>
/// 產品名稱值物件 - 封裝產品名稱驗證邏輯
/// </summary>
public record ProductName
{
    public string Value { get; init; }

    // 私有建構函式（用於 Dapper）
    private ProductName()
    {
        Value = string.Empty;
    }

    public ProductName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Product name cannot be empty");

        if (value.Length > ValidationRules.ProductName.MaxLength)
            throw new DomainException($"Product name cannot exceed {ValidationRules.ProductName.MaxLength} characters");

        Value = value;
    }

    // 隱式轉換：方便與 string 互換
    public static implicit operator string(ProductName name) => name.Value;
    public static implicit operator ProductName(string value) => new(value);

    public override string ToString() => Value;
}
