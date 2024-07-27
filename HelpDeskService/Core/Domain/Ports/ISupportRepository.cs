namespace Domain.Ports;
public interface ISupportRepository
{
    public Task<Entities.Support> CreateAsync(Entities.Support support);
    public Task<Entities.Support?> GetOneByIdAsync(Guid id);
    public Task<Entities.Support?> GetOneByEmailAsync(string email);
    public Task<List<Entities.Support>> GetAllAsync(int perPage, int page, string orderBy, string order);
    public Task<Entities.Support> UpdateAsync(Entities.Support support);
    public Task DeleteAsync(Entities.Support support);
}
