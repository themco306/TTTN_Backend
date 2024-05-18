using backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Repositories.IRepositories
{
    public interface ICouponUsageRepository
    {
        Task<List<CouponUsage>> GetByIdAsync(string userId ,long couponId);
        Task<List<CouponUsage>> GetAllAsync();
        Task AddAsync(CouponUsage couponUsage);
        Task DeleteAsync(long id);
    }
}
