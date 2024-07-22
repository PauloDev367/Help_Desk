using Domain.Entities;
using Domain.Ports;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

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

    public async Task<List<Ticket>> GetAllFromUserAsync(Guid userId, int perPage, int page, string orderBy,
        string order)
    {
        IQueryable<Domain.Entities.Ticket> query = _context.Tickets;
        var totalCount = await query.CountAsync();
        int skipAmount = page * perPage;
        query = query
            .Where(x => x.ClientId.Equals(userId))
            .Include(x=>x.Client)
            .OrderBy(orderBy + " " + order)
            .Skip(skipAmount)
            .Take(perPage);

        var totalPages = (int)Math.Ceiling((double)totalCount / perPage);
        var currentPage = page + 1;
        var nextPage = currentPage < totalPages ? currentPage + 1 : 1;
        var prevPage = currentPage > 1 ? currentPage - 1 : 1;

        var data = await query.AsNoTracking().ToListAsync();
        return data;
    }
}