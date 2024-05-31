using backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Repositories.IRepositories
{
    public interface IMessageRepository
    {
        Task<Message> GetByIdAsync(long id);
        Task<List<Message>> GetAllAsync();
        Task AddAsync(Message message);
        Task UpdateAsync(Message message);
        Task DeleteAsync(long id);
        Task<List<Message>> GetMessagesForUserAsync(string userId);
        // Task<List<Message>> GetUnreadMessagesForUserAsync(string userId);
    }

}
