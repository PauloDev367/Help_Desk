using Application.Dto;
using Application.Exceptions;
using Application.Ticket.Ports;
using Application.Ticket.Request;
using Application.Ticket.Response;
using Domain.Enums;
using Domain.Ports;

namespace Application.Ticket;

public class TicketManager : ITicketManager
{
    private readonly IClientRepository _clientRepository;
    private readonly ITicketRepository _ticketRepository;

    public TicketManager(IClientRepository clientRepository, ITicketRepository ticketRepository)
    {
        _clientRepository = clientRepository;
        _ticketRepository = ticketRepository;
    }

    public async Task<CreatedTicketResponse> CreateAsync(CreateTicketRequest request)
    {
        var client = await _clientRepository.GetOneByIdAsync(request.ClientId);
        if (client == null)
            throw new ClientNotFoundException("Client was not founded");

        var ticket = new Domain.Entities.Ticket
        {
            Title = request.Title,
            TicketStatus = TicketStatus.New,
        };
        ticket.SetClient(client);

        var created = await _ticketRepository.CreateAsync(ticket);
        var response = new CreatedTicketResponse
        {
            Id = created.Id,
            Title = created.Title,
            TicketStatus = created.TicketStatus.ToString(),
            ClientDto = new ClientDto(client)
        };

        return response;
    }

    public async Task<PaginatedClientTicketsResponse> GetClientTicketsAsync(GetTicketFromUserRequest request, Guid clientId)
    {
        var client = await _clientRepository.GetOneByIdAsync(clientId);
        if (client == null) throw new ClientNotFoundException("Client was not founded");
        string[] orderParams = !string.IsNullOrEmpty(request.OrderBy) ? request.OrderBy.ToString().Split(",") : "id,desc".Split(",");
        var orderBy = orderParams[0];
        var order = orderParams[1];
        var data = await _ticketRepository.GetAllFromUserAsync(clientId, request.PerPage, request.Page, orderBy, order);
        return new PaginatedClientTicketsResponse
        {
            PerPage = request.PerPage,
            Page = request.Page,
            Tickets = data.Select(x=>new TicketDto(x)).ToList(),
        };
    }
}