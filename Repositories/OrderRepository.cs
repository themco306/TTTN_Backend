
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
public async Task<List<Order>> GetOrdersBetweenDatesAsync(DateTime startDate, DateTime endDate, List<OrderStatus> statuses)
{
    return await _context.Orders
        .Where(o =>statuses.Contains(o.Status)&& o.CreatedAt >= startDate && o.CreatedAt <= endDate )
        .ToListAsync();
}

        public async Task<Order> GetByIdAsync(long id)
        {

            return await _context.Orders.Include(c=>c.PaidOrder).Include(o=>o.User).Include(o=>o.OrderInfo).Include(o=>o.OrderDetails).ThenInclude(c=>c.Product).ThenInclude(c=>c.Galleries).FirstOrDefaultAsync(c=>c.Id==id);
        }
        public async Task<Order> GetByCodeAsync(string code)
        {

            return await _context.Orders.Include(c=>c.PaidOrder).Include(o=>o.User).Include(o=>o.OrderInfo).Include(o=>o.OrderDetails).ThenInclude(c=>c.Product).ThenInclude(c=>c.Galleries).FirstOrDefaultAsync(c=>c.Code==code);
        }
        public async Task<List<Order>> GetAllAsync()
        {
            return await _context.Orders.Include(o=>o.User).Include(o=>o.OrderInfo).Include(o=>o.OrderDetails).ThenInclude(c=>c.Product).OrderByDescending(c=>c.CreatedAt)
            .ToListAsync();
        }
                public async Task<List<Order>> GetReceivedOrderByUserIdAsync(string userId)
        {
            return await _context.Orders.Where(c=>c.UserId==userId&&c.Status==OrderStatus.Received).Include(o=>o.OrderDetails).ThenInclude(c=>c.Product)
            .ToListAsync();
        }
        public async Task<int> GetTotalOrderCountAsync(string userId)
{
    return await _context.Orders.CountAsync(c => c.UserId == userId);
}

public async Task<List<Order>> GetMyOrdersAsync(string userId, int page, int pageSize)
{
    return await _context.Orders
        .Where(c => c.UserId == userId)
        // .Include(c=>c.PaidOrder)
        .Include(o => o.User)
        .Include(o => o.OrderInfo)
        .Include(o => o.OrderDetails)
        .ThenInclude(c => c.Product)
        .ThenInclude(c=>c.Galleries)
        .OrderByDescending(c=>c.CreatedAt)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        
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
