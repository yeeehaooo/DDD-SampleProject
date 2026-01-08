using SampleProject.Domain.Exceptions;

namespace SampleProject.Domain.Entities;

public class Inventory
{
    public int SkuId { get; private set; }
    public int StorageId { get; private set; }
    public int Quantity { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // 私有建構函式（用於 Dapper）
    private Inventory() { }

    // 公開建構函式（業務邏輯）
    public Inventory(int skuId, int storageId, int quantity = 0)
    {
        SkuId = skuId;
        StorageId = storageId;
        Quantity = quantity;
        CreatedAt = DateTime.UtcNow;

        Validate();
    }

    public void IncreaseQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero");

        Quantity += quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void DecreaseQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero");

        if (Quantity < quantity)
            throw new DomainException("Insufficient inventory");

        Quantity -= quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetQuantity(int quantity)
    {
        if (quantity < 0)
            throw new DomainException("Quantity cannot be negative");

        Quantity = quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    private void Validate()
    {
        if (Quantity < 0)
            throw new DomainException("Quantity cannot be negative");
    }
}
