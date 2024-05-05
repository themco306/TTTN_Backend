
using backend.Context;
using backend.Models;
using backend.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly AppDbContext _context;

        public OrderDetailRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OrderDetail> GetByIdAsync(long id)
        {
            return await _context.OrderDetails.FindAsync(id);
        }

        public async Task<List<OrderDetail>> GetAllAsync()
        {
            return await _context.OrderDetails
            .ToListAsync();
        }

        public async Task AddAsync(OrderDetail orderinfo)
        {
            _context.OrderDetails.Add(orderinfo);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(OrderDetail orderinfo)
        {
            _context.Entry(orderinfo).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var orderinfo = await _context.OrderDetails.FindAsync(id);
            if (orderinfo != null)
            {
                _context.OrderDetails.Remove(orderinfo);
                await _context.SaveChangesAsync();
            }
        }

    }
}
