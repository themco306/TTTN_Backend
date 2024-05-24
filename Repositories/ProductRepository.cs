using backend.Context;
using backend.DTOs;
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
        .Include(p=>p.Brand)
        .Include(p => p.Galleries)
        .FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<Product> GetBySlugAsync(string slug)
        {
            return await _context.Products
                .Include(c=>c.CreatedBy)
        .Include(c=>c.UpdatedBy)
        .Include(p => p.Category)
        .Include(p=>p.Brand)
        .Include(p => p.Galleries)
        .FirstOrDefaultAsync(p => p.Slug == slug);
        }
public async Task<List<Product>> GetAllAsync()
{
    return await _context.Products
    .Include(c=>c.Category)
        .Include(c => c.Galleries.OrderBy(g => g.Order))
        .OrderByDescending(p => p.UpdatedAt)
        .ToListAsync();
}
public async Task<List<Product>> GetSameProductsAsync(long productId, long categoryId, long brandId)
{
    // Get 4 products by category
    var categoryProducts = await _context.Products
        .Where(c => c.CategoryId == categoryId && c.Id != productId&&c.Status==1)
        .Include(c => c.Category)
        .Include(c => c.Brand)
        .Include(c => c.Galleries.OrderBy(g => g.Order))
        .OrderByDescending(p => p.UpdatedAt)
        .Take(4)
        .ToListAsync();

    // Get 4 products by brand
    var brandProducts = await _context.Products
        .Where(c => c.BrandId == brandId && c.Id != productId&&c.Status==1)
        .Include(c => c.Category)
        .Include(c => c.Brand)
        .Include(c => c.Galleries.OrderBy(g => g.Order))
        .OrderByDescending(p => p.UpdatedAt)
        .Take(4)
        .ToListAsync();

    // Combine the two lists without duplicates
    var combinedProducts = new HashSet<Product>(categoryProducts);
    combinedProducts.UnionWith(brandProducts);

    return combinedProducts.ToList();
}
public async Task<PagedResult<Product>> GetFilteredProductsAsync(ProductFilterDTO filter)
{
    var query = _context.Products
        .Include(c => c.Category)
        .Include(c => c.Brand)
        .Include(c => c.Galleries.OrderBy(g => g.Order))
        .AsQueryable();

    if (filter.CategoryId.HasValue)
    {
        query = query.Where(p => p.CategoryId == filter.CategoryId.Value);
    }

    if (filter.BrandId.HasValue)
    {
        query = query.Where(p => p.BrandId == filter.BrandId.Value);
    }

    if (filter.MinPrice.HasValue)
    {
        query = query.Where(p => p.SalePrice >= filter.MinPrice.Value);
    }

    if (filter.MaxPrice.HasValue)
    {
        query = query.Where(p => p.SalePrice <= filter.MaxPrice.Value);
    }

    if (!string.IsNullOrEmpty(filter.SortBy))
    {
        switch (filter.SortBy.ToLower())
        {
            case "name":
                query = filter.SortOrder == "asc" ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name);
                break;
            case "price":
                query = filter.SortOrder == "asc" ? query.OrderBy(p => p.SalePrice) : query.OrderByDescending(p => p.SalePrice);
                break;
            case "rating":
                query = filter.SortOrder == "asc" ? query.OrderByDescending(p => p.TotalItemsSold) : query.OrderBy(p => p.TotalItemsSold);
                break;
            case "date":
                query = filter.SortOrder == "asc" ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt);
                break;
            default:
                query = query.OrderByDescending(p => p.UpdatedAt);
                break;
        }
    }
    else
    {
        query = query.OrderByDescending(p => p.UpdatedAt);
    }

    var totalItems = await query.CountAsync();

    var items = await query
        .Skip((filter.PageNumber - 1) * filter.PageSize)
        .Take(filter.PageSize)
        .ToListAsync();

    return new PagedResult<Product>
    {
        Items = items,
        TotalCount = totalItems,
        PageSize = filter.PageSize,
        CurrentPage = filter.PageNumber
    };
}
   public async Task<List<Product>> GetProductsByTagTypeAsync(TagType type){
    if(type==TagType.BestSeller)
    {
        return await _context.Products
        .Where(p => p.ProductTags.Any(pt => pt.Tag.Type == type)&&p.Status==1)
        .Include(c=>c.Category)
        .Include(c => c.Galleries.OrderBy(g => g.Order))
        .OrderByDescending(p => p.TotalItemsSold)
        .ToListAsync();
        }else{
             return await _context.Products
        .Where(p => p.ProductTags.Any(pt => pt.Tag.Type == type)&&p.Status==1)
        .Include(c=>c.Category)
        .Include(c => c.Galleries.OrderBy(g => g.Order))
        .OrderByDescending(p => p.CreatedAt)
        .ToListAsync();
        }
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
