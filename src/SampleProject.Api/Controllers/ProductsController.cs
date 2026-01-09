using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SampleProject.Application.Commands.Product;
using SampleProject.Application.DTOs;
using SampleProject.Application.Mediator;
using SampleProject.Application.Queries.Product;
using SampleProject.Application.Queries.Sku;
using SampleProject.Api.Helpers;

namespace SampleProject.Api.Controllers;

[ApiController]
[Route("api/products")]
[Tags("Products")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateProductCommand> _createValidator;

    public ProductsController(
        IMediator mediator,
        IValidator<CreateProductCommand> createValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
    }

    /// <summary>
    /// 取得所有商品
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProductDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
    {
        var query = new GetAllProductsQuery();
        var result = await _mediator.SendAsync<IEnumerable<ProductDto>>(query);
        return ApiResponseHelper.Ok(result);
    }

    /// <summary>
    /// 根據 ProductId 取得商品
    /// </summary>
    [HttpGet("{productId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetById(Guid productId)
    {
        var query = new GetProductByIdQuery(productId);
        var result = await _mediator.SendAsync<ProductDto?>(query);

        if (result == null)
            return ApiResponseHelper.NotFound($"Product with ProductId {productId} not found", ErrorCodes.NotFound.ProductNotFound);

        return ApiResponseHelper.Ok(result);
    }

    /// <summary>
    /// 分頁取得商品
    /// </summary>
    [HttpGet("page/{pageNumber:int}/{pageSize:int}")]
    [ProducesResponseType(typeof(ApiResponse<PagedResultDto<ProductDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResultDto<ProductDto>>> GetByPage(
        int pageNumber,
        int pageSize)
    {
        if (pageNumber < 1 || pageSize < 1)
            return ApiResponseHelper.BadRequest("Page number and page size must be greater than 0", ErrorCodes.Pagination.InvalidPageNumber);

        var query = new GetProductsByPageQuery(pageNumber, pageSize);
        var result = await _mediator.SendAsync<PagedResultDto<ProductDto>>(query);
        return ApiResponseHelper.Ok(result);
    }

    /// <summary>
    /// 取得商品的所有 SKU
    /// </summary>
    [HttpGet("{productId:guid}/skus")]
    [ProducesResponseType(typeof(ApiResponse<List<SkuDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<SkuDto>>> GetSkusByProductId(Guid productId)
    {
        // 先取得 Product 以取得內部 Id
        var productQuery = new GetProductByIdQuery(productId);
        var product = await _mediator.SendAsync<ProductDto?>(productQuery);
        if (product == null)
            return ApiResponseHelper.NotFound($"Product with ProductId {productId} not found", ErrorCodes.NotFound.ProductNotFound);

        var query = new GetSkusByProductIdQuery(product.Id);
        var result = await _mediator.SendAsync<List<SkuDto>>(query);
        return ApiResponseHelper.Ok(result);
    }

    /// <summary>
    /// 建立新商品
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductCommand command)
    {
        var validationResult = await _createValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
            return ApiResponseHelper.BadRequest(validationResult);
        }

        var result = await _mediator.SendAsync<ProductDto>(command);
        return ApiResponseHelper.Created(
            result,
            nameof(GetById),
            new { productId = result.ProductId },
            "Product created successfully");
    }

    /// <summary>
    /// 更新商品
    /// </summary>
    [HttpPut("{productId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> Update(
        Guid productId,
        [FromBody] UpdateProductCommand command)
    {
        var updateCommand = command with { ProductId = productId };
        var result = await _mediator.SendAsync<ProductDto>(updateCommand);
        return ApiResponseHelper.Ok(result, "Product updated successfully");
    }

    /// <summary>
    /// 刪除商品
    /// </summary>
    [HttpDelete("{productId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid productId)
    {
        var command = new DeleteProductCommand(productId);
        await _mediator.SendAsync(command);
        return ApiResponseHelper.NoContent();
    }
}
