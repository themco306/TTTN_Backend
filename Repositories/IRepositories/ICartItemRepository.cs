using backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Repositories.IRepositories
{
    public interface ICartItemRepository
    {
        Task<List<CartItem>> GetAllAsync();
        Task<CartItem> GetByIdAsync(long id);
        Task<CartItem> ExitingdAsync(long cartId,long productId);
        Task AddAsync(CartItem cartItem);
        Task UpdateAsync(CartItem cartItem);
        Task DeleteAsync(long id);
        Task DeleteAsync(long cartId,long productId);
    }
}
