using SampleProject.Domain.Exceptions;
using SampleProject.Domain.ValueObjects;

namespace SampleProject.Domain.Entities;

public class Storage
{
    public int Id { get; private set; }
    public Guid StorageId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public Address? Address { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // 私有建構函式（用於 Dapper）
    private Storage() { }

    // 公開建構函式（業務邏輯）- 使用字串地址（向後相容）
    public Storage(string name, string? address = null)
    {
        StorageId = Guid.NewGuid();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Address = string.IsNullOrWhiteSpace(address) ? null : new Address(address);
        IsActive = true;
        CreatedAt = DateTime.UtcNow;

        Validate();
    }

    // 使用 Value Objects 的建構函式
    public Storage(string name, Address? address)
    {
        StorageId = Guid.NewGuid();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Address = address;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;

        Validate();
    }

    public void UpdateName(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Validate();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateAddress(string? address)
    {
        Address = string.IsNullOrWhiteSpace(address) ? null : new Address(address);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateAddress(Address? address)
    {
        Address = address;
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
        if (string.IsNullOrWhiteSpace(Name))
            throw new DomainException("Storage name cannot be empty");

        if (Name.Length > 200)
            throw new DomainException("Storage name cannot exceed 200 characters");
    }
}
