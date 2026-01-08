using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SampleProject.Application.Commands.Product;
using SampleProject.Application.DTOs;
using SampleProject.Application.Mediator;
using SampleProject.Application.Queries.Product;
using SampleProject.Application.Queries.Sku;

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
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
    {
        var query = new GetAllProductsQuery();
        var result = await _mediator.SendAsync<IEnumerable<ProductDto>>(query);
        return Ok(result);
    }

    /// <summary>
    /// 根據 ProductId 取得商品
    /// </summary>
    [HttpGet("{productId:guid}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetById(Guid productId)
    {
        var query = new GetProductByIdQuery(productId);
        var result = await _mediator.SendAsync<ProductDto?>(query);

        if (result == null)
            return NotFound(new { message = $"Product with ProductId {productId} not found" });

        return Ok(result);
    }

    /// <summary>
    /// 分頁取得商品
    /// </summary>
    [HttpGet("page/{pageNumber:int}/{pageSize:int}")]
    [ProducesResponseType(typeof(PagedResultDto<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResultDto<ProductDto>>> GetByPage(
        int pageNumber,
        int pageSize)
    {
        if (pageNumber < 1 || pageSize < 1)
            return BadRequest(new { message = "Page number and page size must be greater than 0" });

        var query = new GetProductsByPageQuery(pageNumber, pageSize);
        var result = await _mediator.SendAsync<PagedResultDto<ProductDto>>(query);
        return Ok(result);
    }

    /// <summary>
    /// 取得商品的所有 SKU
    /// </summary>
    [HttpGet("{productId:guid}/skus")]
    [ProducesResponseType(typeof(List<SkuDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<SkuDto>>> GetSkusByProductId(Guid productId)
    {
        // 先取得 Product 以取得內部 Id
        var productQuery = new GetProductByIdQuery(productId);
        var product = await _mediator.SendAsync<ProductDto?>(productQuery);
        if (product == null)
            return NotFound(new { message = $"Product with ProductId {productId} not found" });

        var query = new GetSkusByProductIdQuery(product.Id);
        var result = await _mediator.SendAsync<List<SkuDto>>(query);
        return Ok(result);
    }

    /// <summary>
    /// 建立新商品
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductCommand command)
    {
        var validationResult = await _createValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
            return BadRequest(new { errors = validationResult.Errors });
        }

        var result = await _mediator.SendAsync<ProductDto>(command);
        return CreatedAtAction(
            nameof(GetById),
            new { productId = result.ProductId },
            result);
    }

    /// <summary>
    /// 更新商品
    /// </summary>
    [HttpPut("{productId:guid}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> Update(
        Guid productId,
        [FromBody] UpdateProductCommand command)
    {
        var updateCommand = command with { ProductId = productId };
        var result = await _mediator.SendAsync<ProductDto>(updateCommand);
        return Ok(result);
    }

    /// <summary>
    /// 刪除商品
    /// </summary>
    [HttpDelete("{productId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid productId)
    {
        var command = new DeleteProductCommand(productId);
        await _mediator.SendAsync(command);
        return NoContent();
    }
}
