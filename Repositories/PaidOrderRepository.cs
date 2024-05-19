
using backend.Context;
using backend.Models;
using backend.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class PaidOrderRepository : IPaidOrderRepository
    {
        private readonly AppDbContext _context;

        public PaidOrderRepository(AppDbContext context)
        {
            _context = context;
        }


        public async Task<PaidOrder> GetByOrderCodeAsync(string orderCode)
        {
            return await _context.PaidOrders
                .FirstOrDefaultAsync(c => c.Order.Code == orderCode);
        }
                public async Task<PaidOrder> GetByPaymentMethodCodeAsync(string code)
        {
            return await _context.PaidOrders
                .FirstOrDefaultAsync(c => c.PaymentMethodCode == code);
        }
        public async Task<PaidOrder> GetFirstAsync()
        {
            return await _context.PaidOrders
            .FirstOrDefaultAsync();
        }
        public async Task AddAsync(PaidOrder paidOrder)
        {
            _context.PaidOrders.Add(paidOrder);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(PaidOrder paidOrder)
        {
            _context.Entry(paidOrder).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

    }
}
