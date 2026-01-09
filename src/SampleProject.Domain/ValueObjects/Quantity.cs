using SampleProject.Domain.Exceptions;

namespace SampleProject.Domain.ValueObjects;

/// <summary>
/// 數量值物件 - 封裝數量驗證和運算邏輯
/// </summary>
public record Quantity
{
    public int Value { get; init; }

    // 私有建構函式（用於 Dapper）
    private Quantity() { }

    public Quantity(int value)
    {
        if (value < ValidationRules.Quantity.MinValue)
            throw new DomainException($"Quantity cannot be negative. Minimum: {ValidationRules.Quantity.MinValue}");

        Value = value;
    }

    public Quantity Add(Quantity other)
    {
        return new Quantity(Value + other.Value);
    }

    public Quantity Subtract(Quantity other)
    {
        if (Value < other.Value)
            throw new DomainException($"Insufficient quantity. Current: {Value}, Requested: {other.Value}");

        return new Quantity(Value - other.Value);
    }

    public Quantity Add(int amount)
    {
        if (amount < 0)
            throw new DomainException("Amount to add cannot be negative");

        return new Quantity(Value + amount);
    }

    public Quantity Subtract(int amount)
    {
        if (amount < 0)
            throw new DomainException("Amount to subtract cannot be negative");

        if (Value < amount)
            throw new DomainException($"Insufficient quantity. Current: {Value}, Requested: {amount}");

        return new Quantity(Value - amount);
    }

    public bool IsGreaterThan(Quantity other) => Value > other.Value;
    public bool IsLessThan(Quantity other) => Value < other.Value;
    public bool IsZero => Value == 0;
    public bool IsPositive => Value > 0;

    public static Quantity Zero => new(0);

    // 隱式轉換：方便與 int 互換
    public static implicit operator int(Quantity quantity) => quantity.Value;
    public static implicit operator Quantity(int value) => new(value);

    public override string ToString() => Value.ToString();
}
