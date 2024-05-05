using backend.Context;
using backend.Models;
using backend.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Repositories
{
    public class SilderRepository : ISilderRepository
    {
        private readonly AppDbContext _context;

        public SilderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Slider> GetByIdAsync(long id)
        {
            return await _context.Sliders
            .Include(c=>c.CreatedBy)
            .Include(c=>c.UpdatedBy)
            .FirstAsync(p => p.Id == id);
        }

        public async Task<List<Slider>> GetAllAsync()
        {
            return await _context.Sliders
            .OrderByDescending(c=>c.UpdatedAt)
            .ToListAsync();
        }

        public async Task AddAsync(Slider slider)
        {
            _context.Sliders.Add(slider);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Slider slider)
        {
            _context.Entry(slider).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Slider slider)
        {
            if (slider != null)
            {
                _context.Sliders.Remove(slider);
                await _context.SaveChangesAsync();
            }
        }
    }
}
