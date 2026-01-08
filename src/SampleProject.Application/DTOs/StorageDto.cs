namespace SampleProject.Application.DTOs;

public record StorageDto(
    int Id,
    Guid StorageId,
    string Name,
    string? Address,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
