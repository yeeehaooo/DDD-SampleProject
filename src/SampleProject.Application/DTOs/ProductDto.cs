namespace SampleProject.Application.DTOs;

public record ProductDto(
    int Id,
    Guid ProductId,
    string Name,
    string Description,
    decimal BasePrice,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
