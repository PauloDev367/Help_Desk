using Application.Client.Ports;
using Application.Client.Request;
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
        var uri = $"v1/api/client/{created.Id}";
        return Created(uri, created);
    }
}
