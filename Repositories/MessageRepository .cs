using backend.Context;
using backend.Models;
using backend.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly AppDbContext _context;

        public MessageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Message> GetByIdAsync(long id)
        {
            return await _context.Messages
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<List<Message>> GetAllAsync()
        {
            return await _context.Messages
                .Include(m => m.User)
                .OrderBy(m => m.UpdatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Message message)
        {
            _context.Entry(message).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message != null)
            {
                _context.Messages.Remove(message);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Message>> GetMessagesForUserAsync(string userId)
        {
            return await _context.Messages
                .Include(m => m.User)
                .Where(m => m.UserId == userId)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        // public async Task<List<Message>> GetUnreadMessagesForUserAsync(string userId)
        // {
        //     return await _context.Messages
        //         .Include(m => m.User)
        //         .Where(m => !m.MessageReadStatuses.Any(r => r.UserId == userId))
        //         .OrderByDescending(m => m.CreatedAt)
        //         .ToListAsync();
        // }
    }
}
