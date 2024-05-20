using backend.Context;
using backend.Models;
using backend.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Repositories
{
    public class CouponRepository : ICouponRepository
    {
        private readonly AppDbContext _context;

        public CouponRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Coupon> GetByIdAsync(long id)
        {
            return await _context.Coupons
             .Include(c => c.CouponUsages)
                .Include(c => c.CreatedBy)
                .Include(c => c.UpdatedBy)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<Coupon> GetShowAllByIdAsync(long id)
        {
            return await _context.Coupons
                 .Include(c => c.CouponUsages)
            .ThenInclude(cu => cu.User)
        .Include(c => c.CouponUsages)
            .ThenInclude(cu => cu.Order)
                .Include(c => c.CreatedBy)
                .Include(c => c.UpdatedBy)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<Coupon> GetByCodeAsync(string code)
        {
            return await _context.Coupons.Where(c=>c.Code==code).Include(c => c.CouponUsages)
                .FirstOrDefaultAsync();
        }
        public async Task<List<Coupon>> GetAllAsync()
        {
            return await _context.Coupons
                .OrderByDescending(c => c.UpdatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(Coupon coupon)
        {
            _context.Coupons.Add(coupon);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Coupon coupon)
        {
            _context.Entry(coupon).State = EntityState.Modified;
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
