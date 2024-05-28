
using backend.Models;

namespace backend.Repositories.IRepositories
{
    public interface IRateLikeRepository
    {
        Task<RateLike> GetByIdAsync(long rateId,string userId);
        Task<List<RateLike>> GetAllAsync();
        Task<List<RateLike>> GetAllActiveAsync();
        Task AddAsync(RateLike rateLike);
        Task UpdateAsync(RateLike rateLike);
        Task DeleteAsync(long id);

    }
}
