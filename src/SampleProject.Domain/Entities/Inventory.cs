using SampleProject.Domain.Exceptions;
using SampleProject.Domain.ValueObjects;

namespace SampleProject.Domain.Entities;

public class Inventory
{
    public int SkuId { get; private set; }
    public int StorageId { get; private set; }
    public Quantity Quantity { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // 私有建構函式（用於 Dapper）
    private Inventory() { }

    // 公開建構函式（業務邏輯）- 使用 int（向後相容）
    public Inventory(int skuId, int storageId, int quantity = 0)
    {
        SkuId = skuId;
        StorageId = storageId;
        Quantity = new Quantity(quantity);
        CreatedAt = DateTime.UtcNow;
    }

    // 使用 Value Objects 的建構函式
    public Inventory(int skuId, int storageId, Quantity quantity)
    {
        SkuId = skuId;
        StorageId = storageId;
        Quantity = quantity ?? throw new ArgumentNullException(nameof(quantity));
        CreatedAt = DateTime.UtcNow;
    }

    public void IncreaseQuantity(int quantity)
    {
        Quantity = Quantity.Add(quantity);
        UpdatedAt = DateTime.UtcNow;
    }

    public void IncreaseQuantity(Quantity quantity)
    {
        Quantity = Quantity.Add(quantity);
        UpdatedAt = DateTime.UtcNow;
    }

    public void DecreaseQuantity(int quantity)
    {
        Quantity = Quantity.Subtract(quantity);
        UpdatedAt = DateTime.UtcNow;
    }

    public void DecreaseQuantity(Quantity quantity)
    {
        Quantity = Quantity.Subtract(quantity);
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetQuantity(int quantity)
    {
        Quantity = new Quantity(quantity);
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetQuantity(Quantity quantity)
    {
        Quantity = quantity ?? throw new ArgumentNullException(nameof(quantity));
        UpdatedAt = DateTime.UtcNow;
    }
}
