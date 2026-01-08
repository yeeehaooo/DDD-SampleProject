namespace SampleProject.Application.DTOs;

public record SkuDto(
    int Id,
    Guid SkuId,
    int ProductId,
    string SkuCode,
    decimal? Price,
    bool IsActive,
    List<SpecificationValueDto> Specifications,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
