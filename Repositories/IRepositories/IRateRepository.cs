
using backend.Models;

namespace backend.Repositories.IRepositories
{
    public interface IRateRepository
    {
        Task<Rate> GetByIdAsync(long id);
        Task<List<Rate>> GetAllAsync();
        Task<List<Rate>> GetByProductIdAsync(long productId);
        Task<int> CountByProductIdAsync(long productId);
        Task<List<Rate>> GetAllActiveAsync();
        Task<int> CountRateAsync(string userId,long productId);
        Task AddAsync(Rate rate);
        Task UpdateAsync(Rate rate);
        Task DeleteAsync(long id);

    }
}
