using Domain.Entities;

namespace Domain.Ports;
public interface ITicketRepository
{
    public Task<Ticket> CreateAsync(Ticket ticket);
}
