using Microsoft.Extensions.Logging;
using SampleProject.Application.DTOs;
using SampleProject.Application.Mediator;
using SampleProject.Domain.Interfaces;

namespace SampleProject.Application.Commands.Sku;

public class CreateSkuCommandHandler : IRequestHandler<CreateSkuCommand, SkuDto>
{
    private readonly ISkuRepository _skuRepository;
    private readonly IProductRepository _productRepository;
    private readonly ISkuSpecificationRepository _skuSpecificationRepository;
    private readonly ISpecificationValueRepository _specificationValueRepository;
    private readonly ISpecificationRepository _specificationRepository;
    private readonly ILogger<CreateSkuCommandHandler> _logger;

    public CreateSkuCommandHandler(
        ISkuRepository skuRepository,
        IProductRepository productRepository,
        ISkuSpecificationRepository skuSpecificationRepository,
        ISpecificationValueRepository specificationValueRepository,
        ISpecificationRepository specificationRepository,
        ILogger<CreateSkuCommandHandler> logger)
    {
        _skuRepository = skuRepository;
        _productRepository = productRepository;
        _skuSpecificationRepository = skuSpecificationRepository;
        _specificationValueRepository = specificationValueRepository;
        _specificationRepository = specificationRepository;
        _logger = logger;
    }

    public async Task<SkuDto> HandleAsync(
        CreateSkuCommand request,
        CancellationToken cancellationToken = default)
    {
        // 驗證 Product 存在
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null)
        {
            throw new KeyNotFoundException($"Product with Id {request.ProductId} not found");
        }

        // 驗證 SkuCode 唯一性
        var existingSku = await _skuRepository.GetBySkuCodeAsync(request.SkuCode, cancellationToken);
        if (existingSku != null)
        {
            throw new InvalidOperationException($"SkuCode {request.SkuCode} already exists");
        }

        // 建立 SKU
        var sku = new Domain.Entities.Sku(request.ProductId, request.SkuCode, request.Price);
        await _skuRepository.AddAsync(sku, cancellationToken);
        await _skuRepository.SaveChangesAsync(cancellationToken);

        // 建立規格值關聯
        if (request.SpecificationValueIds != null && request.SpecificationValueIds.Any())
        {
            foreach (var specValueId in request.SpecificationValueIds)
            {
                var specValue = await _specificationValueRepository.GetByIdAsync(specValueId, cancellationToken);
                if (specValue == null)
                {
                    throw new KeyNotFoundException($"SpecificationValue with Id {specValueId} not found");
                }

                var skuSpec = new Domain.Entities.SkuSpecification(sku.Id, specValueId);
                await _skuSpecificationRepository.AddAsync(skuSpec, cancellationToken);
            }
            await _skuSpecificationRepository.SaveChangesAsync(cancellationToken);
        }

        // 取得規格值列表
        var specifications = await _skuSpecificationRepository.GetBySkuIdAsync(sku.Id, cancellationToken);
        var specificationDtos = new List<SpecificationValueDto>();

        foreach (var skuSpec in specifications)
        {
            var specValue = await _specificationValueRepository.GetByIdAsync(skuSpec.SpecificationValueId, cancellationToken);
            if (specValue != null)
            {
                // 取得 Specification 名稱
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

        _logger.LogInformation("Sku created with SkuId: {SkuId}", sku.SkuId);

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
