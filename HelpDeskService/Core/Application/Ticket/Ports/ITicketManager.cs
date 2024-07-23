using Application.Dto;
using Application.Ticket.Request;
using Application.Ticket.Response;

namespace Application.Ticket.Ports;

public interface ITicketManager
{
    public Task<CreatedTicketResponse> CreateAsync(CreateTicketRequest request);
    public Task<PaginatedClientTicketsResponse> GetClientTicketsAsync(GetTicketFromUserRequest request, Guid clientId);
    public Task<PaginatedClientTicketsResponse> GetAllTicketsAsync(GetAllTicketsRequest request);
    public Task<TicketDto> GetOneAsync(Guid id);
    public Task<TicketDto> GetOneFromClientAsync(Guid id, Guid clientId);
}