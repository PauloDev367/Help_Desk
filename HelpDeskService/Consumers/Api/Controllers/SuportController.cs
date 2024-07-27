using System.Security.Claims;
using Application.Support.Ports;
using Application.Support.Request;
using Microsoft.AspNetCore.Authorization;
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
    
    
    
    [HttpPut]
    [Authorize(Roles = "Support")]
    public async Task<IActionResult> UpdateAsync([FromBody] UpdateSupportRequest request)
    {
        var id = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var update = await _supportManager.UpdateAsync(request, id);
        return Ok(update);
    }
    [HttpDelete]
    [Authorize(Roles = "Support")]
    public async Task<IActionResult> DeleteAsync()
    {
        var id = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        await _supportManager.DeleteAsync(id);
        return NoContent();
    }
    
    
    
    [HttpGet("id:guid")]
    [Authorize(Roles = "Support")]
    public async Task<IActionResult> GetOneAsync(Guid id)
    {
        var support = await _supportManager.GetOneAsync(id);
        return Ok(support);
    }
    [HttpGet]
    [Authorize(Roles = "Support")]
    public async Task<IActionResult> GetAllAsync([FromQuery] GetSupportRequest request)
    {
        var data = await _supportManager.GetAllAsync(request);
        return Ok(data);
    }
}
