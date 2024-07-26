namespace Domain.Ports;
public interface IClientRepository
{
    public Task<Entities.Client> CreateAsync(Entities.Client client);
    public Task<Entities.Client?> GetOneByIdAsync(Guid id);
    public Task<Entities.Client?> GetOneByEmailAsync(string email);
    public Task<List<Entities.Client>> GetAllAsync(int perPage, int page, string orderBy, string order);
    public Task<Entities.Client> UpdateAsync(Entities.Client client);
    public Task DeleteAsync(Entities.Client client);
}
