using Microsoft.AspNetCore.Mvc;
using SampleProject.Application.Commands.Sku;
using SampleProject.Application.DTOs;
using SampleProject.Application.Mediator;
using SampleProject.Application.Queries.Sku;

namespace SampleProject.Api.Controllers;

[ApiController]
[Route("api/skus")]
[Tags("Skus")]
public class SkusController : ControllerBase
{
    private readonly IMediator _mediator;

    public SkusController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// 根據 SkuId 取得 SKU
    /// </summary>
    [HttpGet("{skuId:guid}")]
    [ProducesResponseType(typeof(SkuDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SkuDto>> GetById(Guid skuId)
    {
        var query = new GetSkuByIdQuery(skuId);
        var result = await _mediator.SendAsync<SkuDto?>(query);

        if (result == null)
            return NotFound(new { message = $"Sku with SkuId {skuId} not found" });

        return Ok(result);
    }

    /// <summary>
    /// 根據 SkuCode 取得 SKU
    /// </summary>
    [HttpGet("code/{skuCode}")]
    [ProducesResponseType(typeof(SkuDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SkuDto>> GetBySkuCode(string skuCode)
    {
        var query = new GetSkuBySkuCodeQuery(skuCode);
        var result = await _mediator.SendAsync<SkuDto?>(query);

        if (result == null)
            return NotFound(new { message = $"Sku with SkuCode {skuCode} not found" });

        return Ok(result);
    }

    /// <summary>
    /// 建立新 SKU
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(SkuDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SkuDto>> Create([FromBody] CreateSkuCommand command)
    {
        var result = await _mediator.SendAsync<SkuDto>(command);
        return CreatedAtAction(
            nameof(GetById),
            new { skuId = result.SkuId },
            result);
    }

    /// <summary>
    /// 更新 SKU
    /// </summary>
    [HttpPut("{skuId:guid}")]
    [ProducesResponseType(typeof(SkuDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SkuDto>> Update(
        Guid skuId,
        [FromBody] UpdateSkuCommand command)
    {
        var updateCommand = command with { SkuId = skuId };
        var result = await _mediator.SendAsync<SkuDto>(updateCommand);
        return Ok(result);
    }

    /// <summary>
    /// 刪除 SKU
    /// </summary>
    [HttpDelete("{skuId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid skuId)
    {
        var command = new DeleteSkuCommand(skuId);
        await _mediator.SendAsync(command);
        return NoContent();
    }
}
