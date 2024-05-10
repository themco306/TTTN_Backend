using backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Repositories.IRepositories
{
    public interface IProductRepository
    {
        Task<Product> GetByIdAsync(long id);
        Task<List<Product>> GetAllAsync();
        Task<List<Product>> GetTopProductsByTotalItemsSoldAsync(int count);
        Task<List<Product>> GetProductsByTagTypeAsync(TagType type);
        Task<Product> GetLastProductByTagTypeAsync(TagType type);
        Task RemoveProductTagAsync(ProductTag productTag);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(long id);
        Task<List<Product>> GetProductsByCategoryIdAsync(long categoryId);
    }
}
