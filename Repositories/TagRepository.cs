
using backend.Context;
using backend.Models;
using backend.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly AppDbContext _context;

        public TagRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Tag> GetByTypeAsync(TagType type)
        {
            return await _context.Tags
                .FirstOrDefaultAsync(c => c.Type == type);
        }
        public async Task<Tag> GetByIdAsync(long id)
        {
            return await _context.Tags
                .FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<List<Tag>> GetAllAsync()
        {
            return await _context.Tags
            .OrderBy(c=>c.Sort)
            .ToListAsync();
        }
        public async Task UpdateAsync(Tag Tag)
        {
            _context.Entry(Tag).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

    }
}
