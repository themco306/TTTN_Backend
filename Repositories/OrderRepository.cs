
using backend.Context;
using backend.Models;
using backend.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Order> GetByIdAsync(long id)
        {

            return await _context.Orders.Include(o=>o.User).Include(o=>o.OrderInfo).Include(o=>o.OrderDetails).FirstOrDefaultAsync(c=>c.Id==id);
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await _context.Orders.Include(o=>o.User).Include(o=>o.OrderInfo).Include(o=>o.OrderDetails)
            .ToListAsync();
        }

        public async Task AddAsync(Order orderinfo)
        {
            _context.Orders.Add(orderinfo);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Order orderinfo)
        {
            _context.Entry(orderinfo).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var orderinfo = await _context.Orders.FindAsync(id);
            if (orderinfo != null)
            {
                _context.Orders.Remove(orderinfo);
                await _context.SaveChangesAsync();
            }
        }

    }
}
