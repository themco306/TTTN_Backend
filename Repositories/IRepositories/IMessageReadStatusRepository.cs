using backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Repositories.IRepositories
{

    public interface IMessageReadStatusRepository
    {
        Task<MessageReadStatus> GetByIdAsync(long id);
        Task AddAsync(MessageReadStatus messageReadStatus);
        Task UpdateAsync(MessageReadStatus messageReadStatus);
        Task DeleteAsync(long id);
        Task MarkMessageAsReadAsync(long messageId, string userId);
        Task<bool> IsMessageReadByUserAsync(long messageId, string userId);
    }
}
