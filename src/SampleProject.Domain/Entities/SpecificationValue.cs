using SampleProject.Domain.Exceptions;

namespace SampleProject.Domain.Entities;

public class SpecificationValue
{
    public int Id { get; private set; }
    public Guid SpecificationValueId { get; private set; }
    public int SpecificationId { get; private set; }
    public string Value { get; private set; } = string.Empty;
    public int DisplayOrder { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // 私有建構函式（用於 Dapper）
    private SpecificationValue() { }

    // 公開建構函式（業務邏輯）
    public SpecificationValue(int specificationId, string value, int displayOrder = 0)
    {
        SpecificationValueId = Guid.NewGuid();
        SpecificationId = specificationId;
        Value = value ?? throw new ArgumentNullException(nameof(value));
        DisplayOrder = displayOrder;
        CreatedAt = DateTime.UtcNow;

        Validate();
    }

    public void UpdateValue(string value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
        Validate();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDisplayOrder(int displayOrder)
    {
        DisplayOrder = displayOrder;
        UpdatedAt = DateTime.UtcNow;
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Value))
            throw new DomainException("SpecificationValue value cannot be empty");

        if (Value.Length > 100)
            throw new DomainException("SpecificationValue value cannot exceed 100 characters");
    }
}
