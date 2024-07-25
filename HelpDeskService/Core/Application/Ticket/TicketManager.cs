using Application.Dto;
using Application.Exceptions;
using Application.Ticket.Ports;
using Application.Ticket.Request;
using Application.Ticket.Response;
using Domain.Entities;
using Domain.Enums;
using Domain.Ports;

namespace Application.Ticket;

public class TicketManager : ITicketManager
{
    private readonly IClientRepository _clientRepository;
    private readonly ISupportRepository _supportRepository;
    private readonly ITicketRepository _ticketRepository;

    public TicketManager(IClientRepository clientRepository, ITicketRepository ticketRepository,
        ISupportRepository supportRepository)
    {
        _clientRepository = clientRepository;
        _ticketRepository = ticketRepository;
        _supportRepository = supportRepository;
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

    public async Task<PaginatedClientTicketsResponse> GetClientTicketsAsync(GetTicketFromUserRequest request,
        Guid clientId)
    {
        var client = await _clientRepository.GetOneByIdAsync(clientId);
        if (client == null) throw new ClientNotFoundException("Client was not founded");
        string[] orderParams = !string.IsNullOrEmpty(request.OrderBy)
            ? request.OrderBy.ToString().Split(",")
            : "id,desc".Split(",");
        var orderBy = orderParams[0];
        var order = orderParams[1];
        var data = await _ticketRepository.GetAllFromUserAsync(clientId, request.PerPage, request.Page, orderBy, order);
        return new PaginatedClientTicketsResponse
        {
            PerPage = request.PerPage,
            Page = request.Page,
            Tickets = data.Select(x => new TicketDto(x)).ToList(),
        };
    }

    public async Task<PaginatedClientTicketsResponse> GetAllTicketsAsync(GetAllTicketsRequest request)
    {
        string[] orderParams = !string.IsNullOrEmpty(request.OrderBy)
            ? request.OrderBy.ToString().Split(",")
            : "id,desc".Split(",");
        var orderBy = orderParams[0];
        var order = orderParams[1];
        var data = await _ticketRepository.GetAllAsync(request.PerPage, request.Page, orderBy, order);
        return new PaginatedClientTicketsResponse
        {
            PerPage = request.PerPage,
            Page = request.Page,
            Tickets = data.Select(x => new TicketDto(x)).ToList(),
        };
    }

    public async Task<TicketDto> GetOneAsync(Guid id)
    {
        var ticket = await _ticketRepository.GetOneAsync(id);
        if (ticket == null) throw new TicketNotFoundedException("Ticket was not founded");
        return new TicketDto(ticket);
    }

    public async Task<TicketDto> GetOneFromClientAsync(Guid id, Guid clientId)
    {
        var ticket = await _ticketRepository.GetOneFromClientAsync(id, clientId);
        if (ticket == null) throw new TicketNotFoundedException("Ticket was not founded");
        return new TicketDto(ticket);
    }

    public async Task<TicketWithoutCommentDto> AddCommentAsync(AddCommentToTicketRequest request)
    {
        var comment = new Domain.Entities.Comment { ClientId = request.ClientId };

        Domain.Entities.Ticket ticket;
        if (request.From == TicketAction.FromClient)
        {
            ticket = await _ticketRepository.GetOneFromClientAsync(request.TicketId, request.ClientId);
            if (ticket == null) throw new TicketNotFoundedException("Ticket was not founded");

            comment.IsClientComment = true;
            comment.Client = ticket.Client;
        }
        else
        {
            ticket = await _ticketRepository.GetOneAsync(request.TicketId);
            if (ticket == null) throw new TicketNotFoundedException("Ticket was not founded");

            if (ticket.SupportId == null)
            {
                var support = await _supportRepository.GetOneByIdAsync(request.SupportId);
                if (support == null) throw new InvalidSupportException("Support was not founded");

                ticket.SetSupport(support);
            }
            else if (ticket.SupportId != request.SupportId)
                throw new InvalidSupportException(
                    "You don't have permission to access this ticket! They already have a support");

            comment.IsClientComment = false;
            comment.Support = ticket.Support;
            comment.SupportId = request.SupportId;
        }

        comment.Text = request.Message;
        ticket.AddComment(comment, request.From);
        await _ticketRepository.UpdateAsync(ticket);
        return new TicketWithoutCommentDto(ticket);
    }

    public async Task<TicketWithoutCommentDto> CancelTicket(Guid ticketId, TicketAction action)
    {
        var ticket = await _ticketRepository.GetOneAsync(ticketId);
        if (ticket == null) throw new TicketNotFoundedException("Ticket was not founded");

        ticket.CancelTicket(action);
        await _ticketRepository.UpdateAsync(ticket);
        return new TicketWithoutCommentDto(ticket);
    }

    public async Task<TicketWithoutCommentDto> FinishTicket()
    {
        throw new NotImplementedException("");
    }

    public async Task<TicketWithoutCommentDto> AddSupportToTicket()
    {
        throw new NotImplementedException("");
    }
}