
using backend.Models;

namespace backend.Repositories.IRepositories
{
    public interface IOrderDetailRepository
    {
        Task<OrderDetail> GetByIdAsync(long id);
        Task<List<OrderDetail>> GetAllAsync();
        Task AddAsync(OrderDetail order);
        Task UpdateAsync(OrderDetail order);
        Task DeleteAsync(long id);
    }
}
