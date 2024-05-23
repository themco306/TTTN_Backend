using backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Repositories.IRepositories
{
    public interface IPostRepository
    {
        Task<Post> GetByIdAsync(long id);
        Task<List<Post>> GetAllAsync(bool active=false);
        Task<List<Post>> GetAllPageAsync(bool active=false);
        Task AddAsync(Post post);
        Task UpdateAsync(Post post);
        Task DeleteAsync(Post post);
    }
}
