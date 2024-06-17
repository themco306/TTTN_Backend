using backend.DTOs;
using backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Repositories.IRepositories
{
    public interface IProductRepository
    {
        Task<Product> GetByIdAsync(long id);
        Task<Product> GetBySlugAsync(string slug);
        Task<PagedResult<Product>> GetFilteredProductsAsync(ProductFilterDTO filter);
        Task<PagedResult<Product>> GetSearchProductsAsync(ProductSearchDTO searchDTO);
        Task<List<Product>> GetAllAsync();
        Task<PagedResult<Product>> GetAllPageAsync(ProductAdminFilterDTO filterDTO);
        Task<List<Product>> GetSameProductsAsync(long productId,long categoryId,long brandId);
        
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
