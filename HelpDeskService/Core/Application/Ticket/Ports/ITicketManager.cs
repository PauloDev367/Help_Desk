using Application.Ticket.Request;

namespace Application.Ticket.Ports;
public interface ITicketManager
{
    public Task CreateAsync(CreateTicketRequest request);
}
