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
    private readonly IAuthUserService _authUserService;

    public TicketManager(IClientRepository clientRepository, ITicketRepository ticketRepository,
        ISupportRepository supportRepository, IAuthUserService authUserService)
    {
        _clientRepository = clientRepository;
        _ticketRepository = ticketRepository;
        _supportRepository = supportRepository;
        _authUserService = authUserService;
    }

    public async Task<CreatedTicketResponse> CreateAsync(CreateTicketRequest request)
    {
        var clientAuth = await _authUserService.GetOneByIdAsync(request.ClientId);
        if (clientAuth == null)
            throw new ClientNotFoundException("Client was not founded");

        var client = await _clientRepository.GetOneByEmailAsync(clientAuth.Email);
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
        var clientAuth = await _authUserService.GetOneByIdAsync(clientId);
        if (clientAuth == null) throw new ClientNotFoundException("Client was not founded");

        var client = await _clientRepository.GetOneByEmailAsync(clientAuth.Email);
        if (client == null) throw new ClientNotFoundException("Client was not founded");

        string[] orderParams = !string.IsNullOrEmpty(request.OrderBy)
            ? request.OrderBy.ToString().Split(",")
            : "id,desc".Split(",");
        var orderBy = orderParams[0];
        var order = orderParams[1];
        var data = await _ticketRepository.GetAllFromUserAsync(client.Id, request.PerPage, request.Page, orderBy,
            order);
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
        var clientAuth = await _authUserService.GetOneByIdAsync(clientId);
        if (clientAuth == null) throw new ClientNotFoundException("Client was not founded");

        var client = await _clientRepository.GetOneByEmailAsync(clientAuth.Email);
        if (client == null) throw new ClientNotFoundException("Client was not founded");

        var ticket = await _ticketRepository.GetOneFromClientAsync(id, client.Id);
        if (ticket == null) throw new TicketNotFoundedException("Ticket was not founded");
        return new TicketDto(ticket);
    }

    public async Task<TicketWithoutCommentDto> AddCommentAsync(AddCommentToTicketRequest request)
    {
        var comment = new Domain.Entities.Comment { ClientId = request.ClientId };

        Domain.Entities.Ticket ticket;
        if (request.From == TicketAction.FromClient)
        {
            var clientAuth = await _authUserService.GetOneByIdAsync(request.ClientId);
            if (clientAuth == null) throw new ClientNotFoundException("Client was not founded");

            var client = await _clientRepository.GetOneByEmailAsync(clientAuth.Email);
            if (client == null) throw new ClientNotFoundException("Client was not founded");

            ticket = await _ticketRepository.GetOneFromClientAsync(request.TicketId, client.Id);
            if (ticket == null) throw new TicketNotFoundedException("Ticket was not founded");

            comment.IsClientComment = true;
            comment.ClientId = ticket.Client.Id;
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

    public async Task<TicketWithoutCommentDto> CancelTicketAsync(Guid ticketId, TicketAction action, Guid clientId)
    {
        Domain.Entities.Ticket ticket;

        if (action == TicketAction.FromClient)
        {
            var clientAuth = await _authUserService.GetOneByIdAsync(clientId);
            if (clientAuth == null) throw new ClientNotFoundException("Client was not founded");

            var client = await _clientRepository.GetOneByEmailAsync(clientAuth.Email);
            if (client == null) throw new ClientNotFoundException("Client was not founded");

            ticket = await _ticketRepository.GetOneFromClientAsync(ticketId, client.Id);
        }
        else
            ticket = await _ticketRepository.GetOneAsync(ticketId);

        if (ticket == null) throw new TicketNotFoundedException("Ticket was not founded");

        ticket.CancelTicket(action);
        await _ticketRepository.UpdateAsync(ticket);

        return new TicketWithoutCommentDto(ticket);
    }

    public async Task<TicketWithoutCommentDto> FinishTicketAsync(Guid ticketId, TicketAction action, Guid clientId)
    {
        Domain.Entities.Ticket ticket;

        if (action == TicketAction.FromClient)
            ticket = await _ticketRepository.GetOneFromClientAsync(ticketId, clientId);
        else
            ticket = await _ticketRepository.GetOneAsync(ticketId);

        if (ticket == null) throw new TicketNotFoundedException("Ticket was not founded");

        ticket.FinishTicket(action);
        await _ticketRepository.UpdateAsync(ticket);

        return new TicketWithoutCommentDto(ticket);
    }

    public async Task<TicketWithoutCommentDto> AddSupportToTicket(Guid supportId, Guid ticketId)
    {
        var ticket = await _ticketRepository.GetOneAsync(ticketId);
        if (ticket == null) throw new TicketNotFoundedException("Ticket was not founded");

        var support = await _supportRepository.GetOneByIdAsync(supportId);
        if (support == null) throw new SupportNotFoundedException("Support was not founded");

        ticket.SetSupport(support);
        var updated = await _ticketRepository.UpdateAsync(ticket);
        return new TicketWithoutCommentDto(updated);
    }
}