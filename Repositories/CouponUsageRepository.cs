using backend.Context;
using backend.Models;
using backend.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Repositories
{
    public class CouponUsageRepository : ICouponUsageRepository
    {
        private readonly AppDbContext _context;

        public CouponUsageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CouponUsage>> GetByIdAsync(string userId ,long couponId)
        {
            return await _context.CouponUsages
                .Where(c => c.UserId == userId&&c.CouponId==couponId).ToListAsync();
        }

        public async Task<List<CouponUsage>> GetAllAsync()
        {
            return await _context.CouponUsages
                .ToListAsync();
        }

        public async Task AddAsync(CouponUsage coupon)
        {
            _context.CouponUsages.Add(coupon);
            await _context.SaveChangesAsync();
        }


        public async Task DeleteAsync(long id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon != null)
            {
                _context.Coupons.Remove(coupon);
                await _context.SaveChangesAsync();
            }
        }
    }
}
