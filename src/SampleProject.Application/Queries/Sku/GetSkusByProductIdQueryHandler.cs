using SampleProject.Application.DTOs;
using SampleProject.Application.Mediator;
using SampleProject.Domain.Interfaces;

namespace SampleProject.Application.Queries.Sku;

public class GetSkusByProductIdQueryHandler : IRequestHandler<GetSkusByProductIdQuery, List<SkuDto>>
{
    private readonly ISkuRepository _skuRepository;
    private readonly ISkuSpecificationRepository _skuSpecificationRepository;
    private readonly ISpecificationValueRepository _specificationValueRepository;
    private readonly ISpecificationRepository _specificationRepository;

    public GetSkusByProductIdQueryHandler(
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

    public async Task<List<SkuDto>> HandleAsync(
        GetSkusByProductIdQuery request,
        CancellationToken cancellationToken = default)
    {
        var skus = await _skuRepository.GetByProductIdAsync(request.ProductId, cancellationToken);
        var result = new List<SkuDto>();

        foreach (var sku in skus)
        {
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

            result.Add(new SkuDto(
                sku.Id,
                sku.SkuId,
                sku.ProductId,
                sku.SkuCode,
                sku.Price,
                sku.IsActive,
                specificationDtos,
                sku.CreatedAt,
                sku.UpdatedAt));
        }

        return result;
    }
}
