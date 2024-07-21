using Application.Support.Ports;
using Application.Support.Request;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("v1/api/supports")]
public class SuportController : ControllerBase
{
    private readonly ISupportManager _supportManager;

    public SuportController(ISupportManager supportManager)
    {
        _supportManager = supportManager;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateSupportRequest request)
    {
        var created = await _supportManager.CreateAsync(request);
        if (created.Errors.Count > 0)
            return BadRequest(created);

        var uri = $"v1/api/suports/{created.Success.Id}";
        return Created(uri, created);
    }
    [HttpPut("id:guid")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateSupportRequest request)
    {
        var update = await _supportManager.UpdateAsync(request, id);
        return Ok(update);
    }
    [HttpDelete("id:guid")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        await _supportManager.DeleteAsync(id);
        return NoContent();
    }
    [HttpGet("id:guid")]
    public async Task<IActionResult> GetOneAsync(Guid id)
    {
        var support = await _supportManager.GetOneAsync(id);
        return Ok(support);
    }
    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] GetSupportRequest request)
    {
        var data = await _supportManager.GetAllAsync(request);
        return Ok(data);
    }
}
