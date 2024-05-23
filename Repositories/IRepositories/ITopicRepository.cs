using backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Repositories.IRepositories
{
    public interface ITopicRepository
    {
        Task<Topic> GetByIdAsync(long id);
        Task<Topic> GetShowAllByIdAsync(long id);
        Task<List<Topic>> GetAllAsync();
        Task<List<Topic>> GetAllActiceAsync();
        Task AddAsync(Topic topic);
        Task UpdateAsync(Topic topic);
        Task DeleteAsync(long id);
        Task<List<Topic>> GetChildCategoriesAsync(long parentId);
    }
}
