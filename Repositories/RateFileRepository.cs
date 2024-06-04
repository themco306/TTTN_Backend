using backend.Context;
using backend.Models;
using backend.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Repositories
{
    public class RateFileRepository : IRateFileRepository
    {
        private readonly AppDbContext _context;

        public RateFileRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RateFile> GetByIdAsync(long id)
        {
            return await _context.RateFiles.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<RateFile>> GetAllAsync()
        {
            return await _context.RateFiles.ToListAsync();
        }

        public async Task AddAsync(RateFile rateFile)
        {
            _context.RateFiles.Add(rateFile);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(RateFile rateFile)
        {
            _context.Entry(rateFile).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var rateFile = await _context.RateFiles.FindAsync(id);
            if (rateFile != null)
            {
                _context.RateFiles.Remove(rateFile);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<RateFile>> GetRateFilesByRateIdAsync(long rateId)
        {
            return await _context.RateFiles.Where(rf => rf.RateId == rateId).ToListAsync();
        }
    }
}
