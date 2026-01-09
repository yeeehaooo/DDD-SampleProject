using SampleProject.Domain.Exceptions;

namespace SampleProject.Domain.ValueObjects;

/// <summary>
/// 金額值物件 - 封裝金額和貨幣資訊
/// </summary>
public record Money
{
    public decimal Amount { get; init; }
    public string Currency { get; init; }

    // 私有建構函式（用於 Dapper）
    private Money()
    {
        Currency = "TWD";
    }

    public Money(decimal amount, string currency = ValidationRules.Money.DefaultCurrency)
    {
        if (amount < ValidationRules.Money.MinAmount)
            throw new DomainException($"Money amount cannot be negative. Minimum: {ValidationRules.Money.MinAmount}");

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentNullException(nameof(currency));

        Amount = amount;
        Currency = currency;
    }

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new DomainException($"Cannot add money with different currencies. Current: {Currency}, Other: {other.Currency}");

        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new DomainException($"Cannot subtract money with different currencies. Current: {Currency}, Other: {other.Currency}");

        if (Amount < other.Amount)
            throw new DomainException("Insufficient money");

        return new Money(Amount - other.Amount, Currency);
    }

    public Money Multiply(decimal multiplier)
    {
        if (multiplier < 0)
            throw new DomainException("Multiplier cannot be negative");

        return new Money(Amount * multiplier, Currency);
    }

    public bool IsGreaterThan(Money other)
    {
        if (Currency != other.Currency)
            throw new DomainException($"Cannot compare money with different currencies. Current: {Currency}, Other: {other.Currency}");

        return Amount > other.Amount;
    }

    public bool IsLessThan(Money other)
    {
        if (Currency != other.Currency)
            throw new DomainException($"Cannot compare money with different currencies. Current: {Currency}, Other: {other.Currency}");

        return Amount < other.Amount;
    }

    public static Money Zero(string currency = "TWD") => new(0, currency);

    // 隱式轉換：方便與 decimal 互換（僅用於相容性，建議明確使用 Money）
    public static implicit operator decimal(Money money) => money.Amount;

    public override string ToString() => $"{Amount:N2} {Currency}";
}
