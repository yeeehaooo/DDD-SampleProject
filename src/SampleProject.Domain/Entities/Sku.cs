using SampleProject.Domain.Exceptions;

namespace SampleProject.Domain.Entities;

public class Sku
{
    public int Id { get; private set; }
    public Guid SkuId { get; private set; }
    public int ProductId { get; private set; }
    public string SkuCode { get; private set; } = string.Empty;
    public decimal? Price { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // 私有建構函式（用於 Dapper）
    private Sku() { }

    // 公開建構函式（業務邏輯）
    public Sku(int productId, string skuCode, decimal? price = null)
    {
        SkuId = Guid.NewGuid();
        ProductId = productId;
        SkuCode = skuCode ?? throw new ArgumentNullException(nameof(skuCode));
        Price = price;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;

        Validate();
    }

    public void UpdatePrice(decimal? price)
    {
        if (price.HasValue && price.Value < 0)
            throw new DomainException("Price cannot be negative");

        Price = price;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(SkuCode))
            throw new DomainException("SkuCode cannot be empty");

        if (SkuCode.Length > 50)
            throw new DomainException("SkuCode cannot exceed 50 characters");

        if (Price.HasValue && Price.Value < 0)
            throw new DomainException("Price cannot be negative");
    }
}
