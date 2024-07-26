using Api.ViewModels;
using Application.Ticket.Ports;
using Application.Ticket.Request;
using Domain.Entities;
using Domain.Enums;
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

    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] GetAllTicketsRequest request)
    {
        var data = await _ticketManager.GetAllTicketsAsync(request);
        return Ok(data);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOneAsync(Guid id)
    {
        var data = await _ticketManager.GetOneAsync(id);
        return Ok(data);
    }

    [HttpGet("client")]
    public async Task<IActionResult> GetClientTicketAsync([FromQuery] GetTicketFromUserRequest request)
    {
        // get from token, just for test
        var clientId = new Guid("bd22cee4-256d-4ce1-292e-08dcaa989f7a");
        var data = await _ticketManager.GetClientTicketsAsync(request, clientId);
        return Ok(data);
    }

    [HttpGet("{id:guid}/client")]
    public async Task<IActionResult> GetOneFromClientAsync(Guid id)
    {
        // get from token, just for test
        var clientId = new Guid("bd22cee4-256d-4ce1-292e-08dcaa989f7a");
        var data = await _ticketManager.GetOneFromClientAsync(id, clientId);
        return Ok(data);
    }

    [HttpPost("client/{id:guid}/comment")]
    public async Task<IActionResult> AddCommentAsync([FromBody] AddMessageVM vm, Guid id)
    {
        var clientId = new Guid("bd22cee4-256d-4ce1-292e-08dcaa989f7a");
        var request = new AddCommentToTicketRequest(id, vm.Message, TicketAction.FromClient, clientId, null);
        var data = await _ticketManager.AddCommentAsync(request);
        return Ok(data);
    }

    [HttpPost("{id:guid}/comment")]
    public async Task<IActionResult> AddCommentFromSupportAsync([FromBody] AddMessageVM vm, Guid id)
    {
        var supportId = new Guid("8f93f01e-bcbf-4641-e7ca-08dcab2eee7c");
        var clientId = new Guid("bd22cee4-256d-4ce1-292e-08dcaa989f7a");
        var request =
            new AddCommentToTicketRequest(id, vm.Message, TicketAction.FromSupport, clientId, supportId: supportId);
        var data = await _ticketManager.AddCommentAsync(request);
        return Ok(data);
    }

    [HttpPatch("client/{id:guid}/cancel")]
    public async Task<IActionResult> ClientCancelTicketAsync(Guid id)
    {
        var clientId = new Guid("bd22cee4-256d-4ce1-292e-08dcaa989f7a");
        var updated = await _ticketManager.CancelTicketAsync(id, TicketAction.FromClient, clientId);
        return Ok(updated);
    }

    [HttpPatch("{id:guid}/cancel")]
    public async Task<IActionResult> SupportCancelTicketAsync(Guid id)
    {
        var updated = await _ticketManager.CancelTicketAsync(id, TicketAction.FromSupport, Guid.NewGuid());
        return Ok(updated);
    }

    [HttpPatch("client/{id:guid}/finish")]
    public async Task<IActionResult> ClientFinishTicketAsync(Guid id)
    {
        var clientId = new Guid("bd22cee4-256d-4ce1-292e-08dcaa989f7a");
        var updated = await _ticketManager.FinishTicketAsync(id, TicketAction.FromClient, clientId);
        return Ok(updated);
    }

    [HttpPatch("{id:guid}/finish")]
    public async Task<IActionResult> SupportFinishTicketAsync(Guid id)
    {
        var updated = await _ticketManager.FinishTicketAsync(id, TicketAction.FromSupport, Guid.NewGuid());
        return Ok(updated);
    }

    [HttpPost("{id:guid}/support")]
    public async Task<IActionResult> AddSupportToTicketAsync(Guid id)
    {
        var supportId = new Guid("8f93f01e-bcbf-4641-e7ca-08dcab2eee7c");
        var updated = await _ticketManager.AddSupportToTicket(supportId, id);
        return Ok(updated);
    }
}