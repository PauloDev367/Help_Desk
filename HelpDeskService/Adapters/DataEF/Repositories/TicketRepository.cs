using Domain.Entities;
using Domain.Ports;

namespace DataEF.Repositories;
public class TicketRepository : Repository, ITicketRepository
{
    public TicketRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Ticket> CreateAsync(Ticket ticket)
    {
        await _context.Tickets.AddAsync(ticket);
        await _context.SaveChangesAsync();
        return ticket;
    }
}
