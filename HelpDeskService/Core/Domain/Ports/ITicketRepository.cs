using Domain.Entities;

namespace Domain.Ports;
public interface ITicketRepository
{
    public Task<Ticket> CreateAsync(Ticket ticket);
    public Task<List<Ticket>> GetAllFromUserAsync(Guid userId,int perPage, int page, string orderBy, string order);
    public Task<List<Ticket>> GetAllAsync(int perPage, int page, string orderBy, string order);
    public Task<Ticket?> GetOneAsync(Guid id);
}
