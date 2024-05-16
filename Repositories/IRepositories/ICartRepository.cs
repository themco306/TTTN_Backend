
using backend.Models;

namespace backend.Repositories.IRepositories
{
    public interface ICartRepository
    {
        Task<Cart> GetByIdAsync(long id);
        Task<Cart> GetByUserIdAsync(string id);
        Task<List<Cart>> GetAllAsync();
        Task AddAsync(Cart category);
        Task DeleteAsync(long id);

    }
}
