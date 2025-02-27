
using backend.Context;
using backend.DTOs;
using backend.Models;
using backend.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class RateRepository : IRateRepository
    {
        private readonly AppDbContext _context;

        public RateRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Rate> GetByIdAsync(long id)
        {
            return await _context.Rates
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
        

        public async Task<List<Rate>> GetAllAsync()
        {
            return await _context.Rates
            .OrderByDescending(c=>c.UpdatedAt)
            .ToListAsync();
        }
                public async Task<List<Rate>> GetByProductIdAsync(long productId)
        {
            return await _context.Rates
            .Where(c=>c.ProductId==productId)
            .Include(c=>c.User)
            .Include(c=>c.RateFiles.OrderByDescending(c=>c.FileType))
            .OrderByDescending(c=>c.CreatedAt)
            .ToListAsync();
        }
        public async Task<PagedResult<Rate>> GetByProductIdAsync(long productId, RateQueryDTO queryDTO)
{
    var query = _context.Rates
        .Where(c => c.ProductId == productId)
        .Include(c => c.User)
        .Include(c => c.RateFiles.OrderByDescending(c => c.FileType))
        .AsQueryable();

    // Apply filtering
    if (queryDTO.Star.HasValue)
    {
        query = query.Where(c => c.Star == queryDTO.Star.Value);
    }

    // Apply sorting
    if (!string.IsNullOrEmpty(queryDTO.SortOrder))
    {
        switch (queryDTO.SortOrder.ToLower())
        {
            case "date-asc":
                query = query.OrderBy(c => c.CreatedAt);
                break;
            case "date-desc":
                query = query.OrderByDescending(c => c.CreatedAt);
                break;
                
            default:
                query = query.OrderByDescending(c => c.CreatedAt); // Default sort order
                break;
        }
    }
    else
    {
        query = query.OrderByDescending(c => c.CreatedAt); // Default sort order
    }

    // Apply pagination
    var totalItems = await query.CountAsync();
    var items = await query
        .Skip((queryDTO.PageNumber - 1) * queryDTO.PageSize)
        .Take(queryDTO.PageSize)
        .ToListAsync();

    return new PagedResult<Rate>
    {
        Items = items,
        TotalCount = totalItems,
        PageSize = queryDTO.PageSize,
        CurrentPage = queryDTO.PageNumber
    };
}

                        public async Task<int> CountByProductIdAsync(long productId)
        {
            return await _context.Rates
            .Where(c=>c.ProductId==productId)
            .CountAsync();
        }
public async Task<int> CountRateAsync(string userId, long productId)
{
    return await _context.Rates
        .Where(c => c.UserId == userId && c.ProductId == productId)
        .CountAsync();
}
        public async Task<List<Rate>> GetAllActiveAsync()
        {
            return await _context.Rates.Where(c=>c.Status==1)
            .OrderByDescending(c=>c.UpdatedAt)
            .ToListAsync();
        }
        public async Task AddAsync(Rate rate)
        {
            _context.Rates.Add(rate);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Rate rate)
        {
            _context.Entry(rate).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var rate = await _context.Rates.FindAsync(id);
            if (rate != null)
            {
                _context.Rates.Remove(rate);
                await _context.SaveChangesAsync();
            }
        }
    }
}
