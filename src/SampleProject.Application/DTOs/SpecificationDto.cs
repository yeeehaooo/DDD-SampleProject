namespace SampleProject.Application.DTOs;

public record SpecificationDto(
    int Id,
    Guid SpecificationId,
    string Name,
    int DisplayOrder,
    List<SpecificationValueDto> Values
);
