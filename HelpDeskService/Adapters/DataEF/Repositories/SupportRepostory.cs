using Domain.Ports;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq.Dynamic.Core;
namespace DataEF.Repositories;
public class SupportRepostory : Repository, ISupportRepository
{
    private readonly IMemoryCache _cache;
    private readonly string _supportGetCacheBaseKey = "supportGetCacheBaseKey";
    public SupportRepostory(AppDbContext context, IMemoryCache cache) : base(context)
    {
        _cache = cache;
    }

    public async Task<Domain.Entities.Support> CreateAsync(Domain.Entities.Support support)
    {
        await _context.Supports.AddAsync(support);
        await _context.SaveChangesAsync();
        return support;
    }

    public async Task DeleteAsync(Domain.Entities.Support support)
    {
        _context.Supports.Remove(support);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Domain.Entities.Support>> GetAllAsync(int perPage, int page, string orderBy, string order)
    {
        var cacheKey = $"{_supportGetCacheBaseKey}{perPage}{page}{orderBy}{order}";
        if (!_cache.TryGetValue(cacheKey, out List<Domain.Entities.Support> data))
        {
            IQueryable<Domain.Entities.Support> query = _context.Supports;
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

    public async Task<Domain.Entities.Support?> GetOneByIdAsync(Guid id)
    {
        return await _context.Supports
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Domain.Entities.Support> UpdateAsync(Domain.Entities.Support support)
    {
        _context.Supports.Update(support);
        await _context.SaveChangesAsync();
        return support;
    }
}
