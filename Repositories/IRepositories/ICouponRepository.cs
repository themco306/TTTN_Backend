using backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Repositories.IRepositories
{
    public interface ICouponRepository
    {
        Task<Coupon> GetByIdAsync(long id);
        Task<Coupon> GetByCodeAsync(string code);
        Task<List<Coupon>> GetAllAsync();
        Task AddAsync(Coupon coupon);
        Task UpdateAsync(Coupon coupon);
        Task DeleteAsync(long id);
    }
}
