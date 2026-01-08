namespace SampleProject.Application.DTOs;

public record InventoryDto(
    int SkuId,
    string SkuCode,
    int StorageId,
    string StorageName,
    int Quantity,
    DateTime? UpdatedAt
);
