
using backend.Context;
using backend.Models;
using backend.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class RateLikeRepository : IRateLikeRepository
    {
        private readonly AppDbContext _context;

        public RateLikeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RateLike> GetByIdAsync(long rateId,string userId)
        {
            return await _context.RateLikes
                .FirstOrDefaultAsync(c => c.RateId == rateId&&c.UserId==userId);
        }

        public async Task<List<RateLike>> GetAllAsync()
        {
            return await _context.RateLikes
            .ToListAsync();
        }
        public async Task<List<RateLike>> GetAllActiveAsync()
        {
            return await _context.RateLikes
            .ToListAsync();
        }
        public async Task AddAsync(RateLike ratelike)
        {
            _context.RateLikes.Add(ratelike);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(RateLike ratelike)
        {
            _context.Entry(ratelike).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var ratelike = await _context.RateLikes.FindAsync(id);
            if (ratelike != null)
            {
                _context.RateLikes.Remove(ratelike);
                await _context.SaveChangesAsync();
            }
        }
    }
}
