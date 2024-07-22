using Application.Ticket.Ports;
using Application.Ticket.Request;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/tickets")]
public class TicketController : ControllerBase
{
    private readonly ITicketManager _ticketManager;

    public TicketController(ITicketManager ticketManager)
    {
        _ticketManager = ticketManager;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateTicketRequest request)
    {
        // get from token, just for test
        var clientId = new Guid("bd22cee4-256d-4ce1-292e-08dcaa989f7a");
        request.SetClientId(clientId);
        var created = await _ticketManager.CreateAsync(request);
        var uri = $"api/v1/tickets/{created.Id}";
        return Created(uri, created);
    }

    [HttpGet("client")]
    public async Task<IActionResult> GetClientTicketAsync([FromQuery] GetTicketFromUserRequest request)
    {
        // get from token, just for test
        var clientId = new Guid("bd22cee4-256d-4ce1-292e-08dcaa989f7a");
        var data = await _ticketManager.GetClientTicketsAsync(request, clientId);
        return Ok(data);
    }
}