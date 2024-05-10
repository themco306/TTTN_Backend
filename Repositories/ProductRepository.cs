using backend.Context;
using backend.Models;
using backend.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Product> GetByIdAsync(long id)
        {
            return await _context.Products
                .Include(c=>c.CreatedBy)
        .Include(c=>c.UpdatedBy)
        .Include(p => p.Category)
        .Include(p => p.Galleries)
        .FirstOrDefaultAsync(p => p.Id == id);
        }

public async Task<List<Product>> GetAllAsync()
{
    return await _context.Products
        .Include(c => c.Galleries.OrderBy(g => g.Order))
        .OrderByDescending(p => p.UpdatedAt)
        .ToListAsync();
}
   public async Task<List<Product>> GetProductsByTagTypeAsync(TagType type){
           return await _context.Products
        .Where(p => p.ProductTags.Any(pt => pt.Tag.Type == type))
        .OrderByDescending(p => p.CreatedAt)
        .ToListAsync();
        }
        public async Task<Product> GetLastProductByTagTypeAsync(TagType type)
{
    return await _context.Products
        .Where(p => p.ProductTags.Any(pt => pt.Tag.Type == type))
        .OrderBy(p => p.CreatedAt)
        .FirstOrDefaultAsync();
}
public async Task RemoveProductTagAsync(ProductTag productTag)
        {
                _context.ProductTags.Remove(productTag);
                await _context.SaveChangesAsync();
        }
public async Task<List<Product>> GetTopProductsByTotalItemsSoldAsync(int count)
        {
            // Lấy ra 'count' sản phẩm có TotalItemsSold cao nhất
            var topProducts = await _context.Products
                .OrderByDescending(p => p.TotalItemsSold)
                .Take(count)
                .ToListAsync();

            return topProducts;
        }
        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Product>> GetProductsByCategoryIdAsync(long categoryId)
        {
            return await _context.Products.Where(p => p.Category.Id == categoryId).ToListAsync();
        }
    }
}
