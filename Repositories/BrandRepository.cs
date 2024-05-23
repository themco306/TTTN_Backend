
using backend.Context;
using backend.Models;
using backend.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class BrandRepository : IBrandRepository
    {
        private readonly AppDbContext _context;

        public BrandRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Brand> GetByIdAsync(long id)
        {
            return await _context.Brands
                .Include(c => c.CreatedBy)
                .Include(c => c.UpdatedBy)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Brand>> GetAllAsync()
        {
            return await _context.Brands
            .OrderByDescending(c=>c.UpdatedAt)
            .ToListAsync();
        }

        public async Task AddAsync(Brand brand)
        {
            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Brand brand)
        {
            _context.Entry(brand).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand != null)
            {
                _context.Brands.Remove(brand);
                await _context.SaveChangesAsync();
            }
        }
    }
}
