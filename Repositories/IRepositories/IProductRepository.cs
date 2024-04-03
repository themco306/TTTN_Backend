using backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Repositories.IRepositories
{
    public interface IProductRepository
    {
        Task<Product> GetByIdAsync(long id);
        Task<List<Product>> GetAllAsync();
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(long id);
        Task<List<Product>> GetProductsByCategoryIdAsync(long categoryId);
    }
}
