using SampleProject.Domain.Exceptions;
using SampleProject.Domain.ValueObjects;

namespace SampleProject.Domain.Entities;

public class Product
{
    public int Id { get; private set; }
    public Guid ProductId { get; private set; }
    public ProductName Name { get; private set; } = null!;
    public string Description { get; private set; } = string.Empty;
    public Money BasePrice { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // 私有建構函式（用於 Dapper）
    private Product() { }

    // 公開建構函式（業務邏輯）
    public Product(string name, string description, decimal basePrice, string currency = "TWD")
    {
        // Id 由資料庫自動生成（IDENTITY）
        ProductId = Guid.NewGuid();
        Name = new ProductName(name);
        Description = description ?? throw new ArgumentNullException(nameof(description));
        BasePrice = new Money(basePrice, currency);
        CreatedAt = DateTime.UtcNow;

        Validate();
    }

    // 使用 Value Objects 的建構函式
    public Product(ProductName name, string description, Money basePrice)
    {
        ProductId = Guid.NewGuid();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        BasePrice = basePrice ?? throw new ArgumentNullException(nameof(basePrice));
        CreatedAt = DateTime.UtcNow;

        Validate();
    }

    public void UpdateName(string name)
    {
        Name = new ProductName(name);
        Validate();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateName(ProductName name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Validate();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDescription(string description)
    {
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Validate();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateBasePrice(decimal basePrice, string currency = "TWD")
    {
        BasePrice = new Money(basePrice, currency);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateBasePrice(Money basePrice)
    {
        BasePrice = basePrice ?? throw new ArgumentNullException(nameof(basePrice));
        UpdatedAt = DateTime.UtcNow;
    }

    private void Validate()
    {
        if (Description.Length > ValidationRules.ProductDescription.MaxLength)
            throw new DomainException($"Product description cannot exceed {ValidationRules.ProductDescription.MaxLength} characters");
    }
}
