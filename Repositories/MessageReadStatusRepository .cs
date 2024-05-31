using backend.Context;
using backend.Models;
using backend.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Repositories
{
    public class MessageReadStatusRepository : IMessageReadStatusRepository
    {
        private readonly AppDbContext _context;

        public MessageReadStatusRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<MessageReadStatus> GetByIdAsync(long id)
        {
            return await _context.MessageReadStatuses
                .Include(m => m.Message)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task AddAsync(MessageReadStatus messageReadStatus)
        {
            _context.MessageReadStatuses.Add(messageReadStatus);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MessageReadStatus messageReadStatus)
        {
            _context.Entry(messageReadStatus).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var messageReadStatus = await _context.MessageReadStatuses.FindAsync(id);
            if (messageReadStatus != null)
            {
                _context.MessageReadStatuses.Remove(messageReadStatus);
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkMessageAsReadAsync(long messageId, string userId)
        {
            var readStatus = new MessageReadStatus
            {
                MessageId = messageId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await AddAsync(readStatus);
        }

        public async Task<bool> IsMessageReadByUserAsync(long messageId, string userId)
        {
            return await _context.MessageReadStatuses
                .AnyAsync(m => m.MessageId == messageId && m.UserId == userId);
        }
    }
}
