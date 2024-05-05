
using backend.Models;

namespace backend.Repositories.IRepositories
{
    public interface IOrderInfoRepository
    {
        Task<OrderInfo> GetByIdAsync(long id);
        Task<List<OrderInfo>> GetAllAsync();
        Task AddAsync(OrderInfo orderinfo);
        Task UpdateAsync(OrderInfo orderinfo);
        Task DeleteAsync(long id);
    }
}
