using SampleProject.Domain.Exceptions;

namespace SampleProject.Domain.Entities;

public class Product
{
    public int Id { get; private set; }
    public Guid ProductId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal BasePrice { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // 私有建構函式（用於 Dapper）
    private Product() { }

    // 公開建構函式（業務邏輯）
    public Product(string name, string description, decimal basePrice)
    {
        // Id 由資料庫自動生成（IDENTITY）
        ProductId = Guid.NewGuid();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        BasePrice = basePrice;
        CreatedAt = DateTime.UtcNow;

        Validate();
    }

    public void UpdateName(string name)
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

    public void UpdateBasePrice(decimal basePrice)
    {
        if (basePrice < 0)
            throw new DomainException("BasePrice cannot be negative");

        BasePrice = basePrice;
        UpdatedAt = DateTime.UtcNow;
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new DomainException("Product name cannot be empty");

        if (Name.Length > 200)
            throw new DomainException("Product name cannot exceed 200 characters");

        if (Description.Length > 1000)
            throw new DomainException("Product description cannot exceed 1000 characters");

        if (BasePrice < 0)
            throw new DomainException("BasePrice cannot be negative");
    }
}
