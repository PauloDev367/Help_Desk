using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Client.Ports;
using Application.Client.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("v1/api/clients")]
public class ClientController : ControllerBase
{
    private readonly IClientManager _clientManager;

    public ClientController(IClientManager clientManager)
    {
        _clientManager = clientManager;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateClientRequest request)
    {
        var created = await _clientManager.CreateAsync(request);
        if (created.Errors.Count > 0)
            return BadRequest(created);
        var uri = $"v1/api/client/{created.Success.Id}";
        return Created(uri, created);
    }

    [Authorize(Roles="Client")]
    [HttpPut]
    public async Task<IActionResult> UpdateAsync([FromBody] UpdateClientRequest request)
    {
        var id = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        var update = await _clientManager.UpdateAsync(request, id);
        return Ok(update);
    }
    
    [Authorize(Roles="Support")]
    [HttpDelete("id:guid")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        await _clientManager.DeleteAsync(id);
        return NoContent();
    }
    [Authorize(Roles="Support")]
    [HttpGet("id:guid")]
    public async Task<IActionResult> GetOneAsync(Guid id)
    {
        var client = await _clientManager.GetOneAsync(id);
        return Ok(client);
    }
    [Authorize(Roles="Support")]
    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] GetClientRequest request)
    {
        var data = await _clientManager.GetAllAsync(request);
        return Ok(data);
    }
}
