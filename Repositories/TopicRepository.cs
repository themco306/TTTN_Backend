using backend.Context;
using backend.Models;
using backend.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Repositories
{
    public class TopicRepository : ITopicRepository
    {
        private readonly AppDbContext _context;

        public TopicRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Topic> GetByIdAsync(long id)
        {
            return await _context.Topics
                .Include(c => c.CreatedBy)
                .Include(c => c.UpdatedBy)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<Topic> GetShowAllByIdAsync(long id)
        {
            return await _context.Topics
                .Include(c => c.CreatedBy)
                .Include(c => c.UpdatedBy)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<List<Topic>> GetAllAsync()
        {
            return await _context.Topics
                .OrderByDescending(c => c.UpdatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(Topic topic)
        {
            _context.Topics.Add(topic);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Topic topic)
        {
            _context.Entry(topic).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var topic = await _context.Topics.FindAsync(id);
            if (topic != null)
            {
                _context.Topics.Remove(topic);
                await _context.SaveChangesAsync();
            }
        }
                 public async Task<List<Topic>> GetChildCategoriesAsync(long parentId)
        {
            return await _context.Topics.Where(c => c.ParentId == parentId).ToListAsync();
        }
    }
}
