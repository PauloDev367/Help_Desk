using Application.Dto;
using Application.Ticket.Request;
using Application.Ticket.Response;
using Domain.Entities;
using Domain.Enums;

namespace Application.Ticket.Ports;

public interface ITicketManager
{
    public Task<CreatedTicketResponse> CreateAsync(CreateTicketRequest request);
    public Task<PaginatedClientTicketsResponse> GetClientTicketsAsync(GetTicketFromUserRequest request, Guid clientId);
    public Task<PaginatedClientTicketsResponse> GetAllTicketsAsync(GetAllTicketsRequest request);
    public Task<TicketDto> GetOneAsync(Guid id);
    public Task<TicketDto> GetOneFromClientAsync(Guid id, Guid clientId);
    public Task<TicketWithoutCommentDto> AddCommentAsync(AddCommentToTicketRequest request);
    public Task<TicketWithoutCommentDto> CancelTicketAsync(Guid ticketId, TicketAction action, Guid clientId);
    public Task<TicketWithoutCommentDto> FinishTicketAsync(Guid ticketId, TicketAction action, Guid clientId);
    public Task<TicketWithoutCommentDto> AddSupportToTicket();
}