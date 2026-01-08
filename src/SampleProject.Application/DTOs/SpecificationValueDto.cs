namespace SampleProject.Application.DTOs;

public record SpecificationValueDto(
    int Id,
    Guid SpecificationValueId,
    int SpecificationId,
    string SpecificationName,
    string Value,
    int DisplayOrder
);
