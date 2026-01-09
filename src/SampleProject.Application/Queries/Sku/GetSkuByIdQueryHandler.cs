using SampleProject.Application.DTOs;
using SampleProject.Application.Mediator;
using SampleProject.Domain.Interfaces;

namespace SampleProject.Application.Queries.Sku;

public class GetSkuByIdQueryHandler : IRequestHandler<GetSkuByIdQuery, SkuDto?>
{
    private readonly ISkuRepository _skuRepository;
    private readonly ISkuSpecificationRepository _skuSpecificationRepository;
    private readonly ISpecificationValueRepository _specificationValueRepository;
    private readonly ISpecificationRepository _specificationRepository;

    public GetSkuByIdQueryHandler(
        ISkuRepository skuRepository,
        ISkuSpecificationRepository skuSpecificationRepository,
        ISpecificationValueRepository specificationValueRepository,
        ISpecificationRepository specificationRepository)
    {
        _skuRepository = skuRepository;
        _skuSpecificationRepository = skuSpecificationRepository;
        _specificationValueRepository = specificationValueRepository;
        _specificationRepository = specificationRepository;
    }

    public async Task<SkuDto?> HandleAsync(
        GetSkuByIdQuery request,
        CancellationToken cancellationToken = default)
    {
        var sku = await _skuRepository.GetBySkuIdAsync(request.SkuId, cancellationToken);
        if (sku == null)
            return null;

        // 取得規格值列表
        var specifications = await _skuSpecificationRepository.GetBySkuIdAsync(sku.Id, cancellationToken);
        var specificationDtos = new List<SpecificationValueDto>();

        foreach (var skuSpec in specifications)
        {
            var specValue = await _specificationValueRepository.GetByIdAsync(skuSpec.SpecificationValueId, cancellationToken);
            if (specValue != null)
            {
                var spec = await _specificationRepository.GetByIdAsync(specValue.SpecificationId, cancellationToken);
                var specName = spec?.Name ?? string.Empty;

                specificationDtos.Add(new SpecificationValueDto(
                    specValue.Id,
                    specValue.SpecificationValueId,
                    specValue.SpecificationId,
                    specName,
                    specValue.Value,
                    specValue.DisplayOrder));
            }
        }

        return new SkuDto(
            sku.Id,
            sku.SkuId,
            sku.ProductId,
            sku.SkuCode,
            sku.Price?.Amount, // 從 Money 提取原始值（可空）
            sku.IsActive,
            specificationDtos,
            sku.CreatedAt,
            sku.UpdatedAt);
    }
}
