using SampleProject.Domain.Exceptions;

namespace SampleProject.Domain.Entities;

public class Specification
{
    public int Id { get; private set; }
    public Guid SpecificationId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int DisplayOrder { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // 私有建構函式（用於 Dapper）
    private Specification() { }

    // 公開建構函式（業務邏輯）
    public Specification(string name, int displayOrder = 0)
    {
        SpecificationId = Guid.NewGuid();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        DisplayOrder = displayOrder;
        CreatedAt = DateTime.UtcNow;

        Validate();
    }

    public void UpdateName(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
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
        if (string.IsNullOrWhiteSpace(Name))
            throw new DomainException("Specification name cannot be empty");

        if (Name.Length > 100)
            throw new DomainException("Specification name cannot exceed 100 characters");
    }
}
