
using backend.Models;

namespace backend.Repositories.IRepositories
{
    public interface IPaidOrderRepository
    {
        Task<PaidOrder> GetByOrderCodeAsync(string orderCode);
        Task<PaidOrder> GetByPaymentMethodCodeAsync(string paymentCode);
        Task<PaidOrder> GetFirstAsync();
        Task AddAsync(PaidOrder paidOrder);
        Task UpdateAsync(PaidOrder paidOrder);
    }
}
