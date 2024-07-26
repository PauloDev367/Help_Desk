using Domain.Ports;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq.Dynamic.Core;

namespace DataEF.Repositories;

public class ClientRepository : Repository, IClientRepository
{
    private readonly IMemoryCache _cache;
    private readonly string _clientGetCacheBaseKey = "clientGetCacheBaseKey";

    public ClientRepository(AppDbContext context, IMemoryCache cache) : base(context)
    {
        _cache = cache;
    }

    public async Task<Domain.Entities.Client> CreateAsync(Domain.Entities.Client client)
    {
        await _context.Clients.AddAsync(client);
        await _context.SaveChangesAsync();
        return client;
    }

    public async Task DeleteAsync(Domain.Entities.Client client)
    {
        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Domain.Entities.Client>> GetAllAsync(int perPage, int page, string orderBy, string order)
    {
        var cacheKey = $"{_clientGetCacheBaseKey}{perPage}{page}{orderBy}{order}";
        if (!_cache.TryGetValue(cacheKey, out List<Domain.Entities.Client> data))
        {
            IQueryable<Domain.Entities.Client> query = _context.Clients;
            var totalCount = await query.CountAsync();
            int skipAmount = page * perPage;
            query = query
                .OrderBy(orderBy + " " + order)
                .Skip(skipAmount)
                .Take(perPage);

            var totalPages = (int)Math.Ceiling((double)totalCount / perPage);
            var currentPage = page + 1;
            var nextPage = currentPage < totalPages ? currentPage + 1 : 1;
            var prevPage = currentPage > 1 ? currentPage - 1 : 1;

            data = await query.AsNoTracking().ToListAsync();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(45))
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(3600))
                .SetPriority(CacheItemPriority.Normal);
            _cache.Set(cacheKey, data, cacheEntryOptions);
        }

        return data;
    }

    public async Task<Domain.Entities.Client?> GetOneByIdAsync(Guid id)
    {
        return await _context.Clients
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Domain.Entities.Client> UpdateAsync(Domain.Entities.Client client)
    {
        _context.Clients.Update(client);
        await _context.SaveChangesAsync();
        return client;
    }

    public async Task<Domain.Entities.Client?> GetOneByEmailAsync(string email)
    {
        return await _context.Clients.FirstOrDefaultAsync(x => x.Email == email);
    }
}