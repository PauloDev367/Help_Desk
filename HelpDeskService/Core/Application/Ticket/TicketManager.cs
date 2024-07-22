using Application.Exceptions;
using Application.Ticket.Ports;
using Application.Ticket.Request;
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

    public async Task CreateAsync(CreateTicketRequest request)
    {
        var client = await _clientRepository.GetOneByIdAsync(request.ClientId);
        if (client == null)
            throw new ClientNotFoundException("Client was not founded");

        var ticket = new Domain.Entities.Ticket
        {
            TicketStatus = request.TicketStatus,
        };
        ticket.SetClient(client);

        await _ticketRepository.CreateAsync(ticket);
    }
}
