using backend.Context;
using backend.Models;
using backend.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Repositories
{
    public class GalleryRepository : IGalleryRepository
    {
        private readonly AppDbContext _context;

        public GalleryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Gallery> GetByIdAsync(long id)
        {
            return await _context.Galleries.FindAsync(id);
        }

        public async Task<List<Gallery>> GetAllAsync()
        {
            return await _context.Galleries.ToListAsync();
        }

        public async Task AddAsync(Gallery gallery)
        {
            _context.Galleries.Add(gallery);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Gallery gallery)
        {
            _context.Entry(gallery).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var gallery = await _context.Galleries.FindAsync(id);
            if (gallery != null)
            {
                _context.Galleries.Remove(gallery);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Gallery>> GetGalleriesByProductIdAsync(long productId)
        {
            return await _context.Galleries.Where(g => g.ProductId == productId).OrderBy(g => g.Order).ToListAsync();
        }
    }
}
