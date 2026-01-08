using Microsoft.Extensions.Logging;
using SampleProject.Application.DTOs;
using SampleProject.Application.Mediator;
using SampleProject.Domain.Interfaces;

namespace SampleProject.Application.Commands.Sku;

public class UpdateSkuCommandHandler : IRequestHandler<UpdateSkuCommand, SkuDto>
{
    private readonly ISkuRepository _skuRepository;
    private readonly ISkuSpecificationRepository _skuSpecificationRepository;
    private readonly ISpecificationValueRepository _specificationValueRepository;
    private readonly ISpecificationRepository _specificationRepository;
    private readonly ILogger<UpdateSkuCommandHandler> _logger;

    public UpdateSkuCommandHandler(
        ISkuRepository skuRepository,
        ISkuSpecificationRepository skuSpecificationRepository,
        ISpecificationValueRepository specificationValueRepository,
        ISpecificationRepository specificationRepository,
        ILogger<UpdateSkuCommandHandler> logger)
    {
        _skuRepository = skuRepository;
        _skuSpecificationRepository = skuSpecificationRepository;
        _specificationValueRepository = specificationValueRepository;
        _specificationRepository = specificationRepository;
        _logger = logger;
    }

    public async Task<SkuDto> HandleAsync(
        UpdateSkuCommand request,
        CancellationToken cancellationToken = default)
    {
        var sku = await _skuRepository.GetBySkuIdAsync(request.SkuId, cancellationToken);
        if (sku == null)
        {
            throw new KeyNotFoundException($"Sku with SkuId {request.SkuId} not found");
        }

        if (request.SkuCode != null)
        {
            var existingSku = await _skuRepository.GetBySkuCodeAsync(request.SkuCode, cancellationToken);
            if (existingSku != null && existingSku.SkuId != request.SkuId)
            {
                throw new InvalidOperationException($"SkuCode {request.SkuCode} already exists");
            }
            // 注意：SkuCode 在 Domain 層沒有 Update 方法，這裡需要透過反射或新增方法
            // 暫時跳過 SkuCode 更新，或需要擴充 Domain 實體
        }

        if (request.Price.HasValue)
        {
            sku.UpdatePrice(request.Price);
        }

        if (request.IsActive.HasValue)
        {
            if (request.IsActive.Value)
                sku.Activate();
            else
                sku.Deactivate();
        }

        await _skuRepository.UpdateAsync(sku, cancellationToken);
        await _skuRepository.SaveChangesAsync(cancellationToken);

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

        _logger.LogInformation("Sku updated with SkuId: {SkuId}", sku.SkuId);

        return new SkuDto(
            sku.Id,
            sku.SkuId,
            sku.ProductId,
            sku.SkuCode,
            sku.Price,
            sku.IsActive,
            specificationDtos,
            sku.CreatedAt,
            sku.UpdatedAt);
    }
}
