using Microsoft.AspNetCore.Mvc;
using SampleProject.Application.Commands.Inventory;
using SampleProject.Application.DTOs;
using SampleProject.Application.Mediator;
using SampleProject.Application.Queries.Inventory;

namespace SampleProject.Api.Controllers;

[ApiController]
[Route("api/inventory")]
[Tags("Inventory")]
public class InventoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// 根據 SkuId 取得庫存
    /// </summary>
    [HttpGet("skus/{skuId:guid}")]
    [ProducesResponseType(typeof(List<InventoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<InventoryDto>>> GetBySkuId(Guid skuId)
    {
        var query = new GetInventoryBySkuIdQuery(skuId);
        var result = await _mediator.SendAsync<List<InventoryDto>>(query);
        return Ok(result);
    }

    /// <summary>
    /// 根據 StorageId 取得庫存
    /// </summary>
    [HttpGet("storages/{storageId:guid}")]
    [ProducesResponseType(typeof(List<InventoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<InventoryDto>>> GetByStorageId(Guid storageId)
    {
        var query = new GetInventoryByStorageIdQuery(storageId);
        var result = await _mediator.SendAsync<List<InventoryDto>>(query);
        return Ok(result);
    }

    /// <summary>
    /// 建立或更新庫存
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(InventoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InventoryDto>> CreateOrUpdate([FromBody] CreateOrUpdateInventoryCommand command)
    {
        var result = await _mediator.SendAsync<InventoryDto>(command);
        return Ok(result);
    }

    /// <summary>
    /// 調整庫存
    /// </summary>
    [HttpPut("adjust")]
    [ProducesResponseType(typeof(InventoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InventoryDto>> Adjust([FromBody] AdjustInventoryCommand command)
    {
        var result = await _mediator.SendAsync<InventoryDto>(command);
        return Ok(result);
    }
}
