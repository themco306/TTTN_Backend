
using backend.Models;

namespace backend.Repositories.IRepositories
{
    public interface IOrderRepository
    {
        Task<Order> GetByIdAsync(long id);
        Task<Order> GetByCodeAsync(string code);
        Task<List<Order>> GetAllAsync();
        Task<int> GetTotalOrderCountAsync(string userId);
        Task<List<Order>> GetMyOrderAsync(string userId,int page=1, int pageSize=5);
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(long id);
    }
}
