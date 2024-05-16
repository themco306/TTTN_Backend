
using backend.Context;
using backend.Models;
using backend.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class OrderInfoRepository : IOrderInfoRepository
    {
        private readonly AppDbContext _context;

        public OrderInfoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OrderInfo> GetByIdAsync(long id)
        {
            return await _context.OrderInfos.FindAsync(id);
        }

        public async Task<List<OrderInfo>> GetAllAsync(string userId)
        {
            return await _context.OrderInfos.Where(c=>c.UserId==userId).Select(o => new OrderInfo
        {
            Id = o.Id,
            UserId=o.UserId,
            DeliveryAddress = o.DeliveryAddress,
            DeliveryProvince=o.DeliveryProvince,
            DeliveryDistrict=o.DeliveryDistrict,
            DeliveryWard=o.DeliveryWard,
            DeliveryName = o.DeliveryName,
            DeliveryPhone = o.DeliveryPhone
        })
            .ToListAsync();
        }

        public async Task AddAsync(OrderInfo orderinfo)
        {
            _context.OrderInfos.Add(orderinfo);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(OrderInfo orderinfo)
        {
            _context.Entry(orderinfo).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var orderinfo = await _context.OrderInfos.FindAsync(id);
            if (orderinfo != null)
            {
                _context.OrderInfos.Remove(orderinfo);
                await _context.SaveChangesAsync();
            }
        }

    }
}
