using Microsoft.AspNetCore.Mvc;
using SampleProject.Application.Commands.Storage;
using SampleProject.Application.DTOs;
using SampleProject.Application.Mediator;
using SampleProject.Application.Queries.Storage;

namespace SampleProject.Api.Controllers;

[ApiController]
[Route("api/storages")]
[Tags("Storages")]
public class StoragesController : ControllerBase
{
    private readonly IMediator _mediator;

    public StoragesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// 取得所有倉庫
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<StorageDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<StorageDto>>> GetAll()
    {
        var query = new GetAllStoragesQuery();
        var result = await _mediator.SendAsync<List<StorageDto>>(query);
        return Ok(result);
    }

    /// <summary>
    /// 取得所有啟用的倉庫
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(List<StorageDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<StorageDto>>> GetActive()
    {
        var query = new GetActiveStoragesQuery();
        var result = await _mediator.SendAsync<List<StorageDto>>(query);
        return Ok(result);
    }

    /// <summary>
    /// 根據 StorageId 取得倉庫
    /// </summary>
    [HttpGet("{storageId:guid}")]
    [ProducesResponseType(typeof(StorageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StorageDto>> GetById(Guid storageId)
    {
        var query = new GetStorageByIdQuery(storageId);
        var result = await _mediator.SendAsync<StorageDto?>(query);

        if (result == null)
            return NotFound(new { message = $"Storage with StorageId {storageId} not found" });

        return Ok(result);
    }

    /// <summary>
    /// 建立新倉庫
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(StorageDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StorageDto>> Create([FromBody] CreateStorageCommand command)
    {
        var result = await _mediator.SendAsync<StorageDto>(command);
        return CreatedAtAction(
            nameof(GetById),
            new { storageId = result.StorageId },
            result);
    }

    /// <summary>
    /// 更新倉庫
    /// </summary>
    [HttpPut("{storageId:guid}")]
    [ProducesResponseType(typeof(StorageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StorageDto>> Update(
        Guid storageId,
        [FromBody] UpdateStorageCommand command)
    {
        var updateCommand = command with { StorageId = storageId };
        var result = await _mediator.SendAsync<StorageDto>(updateCommand);
        return Ok(result);
    }

    /// <summary>
    /// 刪除倉庫
    /// </summary>
    [HttpDelete("{storageId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid storageId)
    {
        var command = new DeleteStorageCommand(storageId);
        await _mediator.SendAsync(command);
        return NoContent();
    }
}
