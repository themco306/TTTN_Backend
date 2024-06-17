using backend.Context;
using backend.DTOs;
using backend.Helper;
using backend.Models;
using backend.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace backend.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        private readonly Generate _generate;
        private string ConvertToUnSign(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            input = input.Trim();
            for (int i = 0x20; i < 0x30; i++)
            {
                input = input.Replace(((char)i).ToString(), " ");
            }

            Regex regex = new Regex(@"\p{IsCombiningDiacriticalMarks}+");
            string str = input.Normalize(NormalizationForm.FormD);
            string str2 = regex.Replace(str, string.Empty).Replace('đ', 'd').Replace('Đ', 'D');

            return str2;
        }

        public ProductRepository(AppDbContext context, Generate generate)
        {
            _context = context;
            _generate = generate;
        }

        public async Task<Product> GetByIdAsync(long id)
        {
            return await _context.Products
                .Include(c => c.CreatedBy)
        .Include(c => c.UpdatedBy)
        .Include(p => p.Category)
        .Include(p => p.Brand)
        .Include(p => p.Galleries)
        .FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<Product> GetBySlugAsync(string slug)
        {
            return await _context.Products
                .Include(c => c.CreatedBy)
        .Include(c => c.UpdatedBy)
        .Include(p => p.Category)
        .Include(p => p.Brand)
        .Include(p => p.Galleries)
        .FirstOrDefaultAsync(p => p.Slug == slug);
        }
        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products
            .Include(c => c.Category)
                .Include(c => c.Galleries.OrderBy(g => g.Order))
                .OrderByDescending(p => p.UpdatedAt)
                .ToListAsync();
        }
        public async Task<PagedResult<Product>> GetAllPageAsync(ProductAdminFilterDTO filterDTO)
        {
            var query = _context.Products
      .Include(p => p.Category)
      .Include(p => p.Brand)
      .Include(p => p.Galleries.OrderBy(g => g.Order))
      .OrderByDescending(p => p.UpdatedAt)
      .AsQueryable();

            if (filterDTO.Status != null)
            {
                query = query.Where(c => c.Status == filterDTO.Status);
            }


            if (!string.IsNullOrEmpty(filterDTO.Key))
            {
                string filter = _generate.GenerateSlug(filterDTO.Key);
                query = query.Where(p => p.Slug.Contains(filter, StringComparison.CurrentCultureIgnoreCase) ||
                p.Category.Slug.Contains(filter, StringComparison.CurrentCultureIgnoreCase)
                ||
                 p.Brand.Slug.Contains(filter, StringComparison.CurrentCultureIgnoreCase));
            }

            if (!string.IsNullOrEmpty(filterDTO.SortOrder))
            {
                switch (filterDTO.SortOrder)
                {
                    case "create-asc":
                        query =  query.OrderByDescending(p => p.CreatedAt);
                        break;
                        case "create-desc":
                        query =  query.OrderBy(p => p.CreatedAt);
                        break;
                    default:
                        break;
                }
            }
            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((filterDTO.PageNumber - 1) * filterDTO.PageSize)
                .Take(filterDTO.PageSize)
                .ToListAsync();

            return new PagedResult<Product>
            {
                Items = items,
                TotalCount = totalItems,
                PageSize = filterDTO.PageSize,
                CurrentPage = filterDTO.PageNumber
            };
        }

        public async Task<List<Product>> GetSameProductsAsync(long productId, long categoryId, long brandId)
        {
            // Get 4 products by category
            var categoryProducts = await _context.Products
                .Where(c => c.CategoryId == categoryId && c.Id != productId && c.Status == 1)
                .Include(c => c.Category)
                .Include(c => c.Brand)
                .Include(c => c.Galleries.OrderBy(g => g.Order))
                .OrderByDescending(p => p.UpdatedAt)
                .Take(4)
                .ToListAsync();

            // Get 4 products by brand
            var brandProducts = await _context.Products
                .Where(c => c.BrandId == brandId && c.Id != productId && c.Status == 1)
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
            .Where(c => c.Status == 1)
                .Include(c => c.Category)
                .Include(c => c.Brand)
                .Include(c => c.Galleries.OrderBy(g => g.Order))
                .AsQueryable();

            if (filter.CategoryId != null)
            {
                query = query.Where(p => p.Category.Slug == filter.CategoryId);
            }

            if (filter.BrandId != null)
            {
                query = query.Where(p => p.Brand.Slug == filter.BrandId);
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
        public async Task<PagedResult<Product>> GetSearchProductsAsync(ProductSearchDTO searchDTO)
        {
            if (string.IsNullOrEmpty(searchDTO.Query))
            {
                return new PagedResult<Product>
                {
                    Items = new List<Product>(),
                    TotalCount = 0,
                    PageSize = searchDTO.PageSize,
                    CurrentPage = searchDTO.PageNumber
                };
            }

            var query = _context.Products
                .Where(c => c.Status == 1)
                .Include(c => c.Category)
                .Include(c => c.Brand)
                .Include(c => c.Galleries.OrderBy(g => g.Order))
                .AsQueryable();
            string filter = _generate.GenerateSlug(searchDTO.Query);

            query = query.Where(p => p.Slug.Contains(filter, StringComparison.CurrentCultureIgnoreCase) ||
                                            p.Category.Slug.Contains(filter, StringComparison.CurrentCultureIgnoreCase)  ||
                 p.Brand.Slug.Contains(filter, StringComparison.CurrentCultureIgnoreCase));


            if (searchDTO.MinPrice.HasValue)
            {
                query = query.Where(p => p.SalePrice >= searchDTO.MinPrice.Value);
            }

            if (searchDTO.MaxPrice.HasValue)
            {
                query = query.Where(p => p.SalePrice <= searchDTO.MaxPrice.Value);
            }

            if (!string.IsNullOrEmpty(searchDTO.SortBy))
            {
                switch (searchDTO.SortBy.ToLower())
                {
                    case "name":
                        query = searchDTO.SortOrder == "asc" ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name);
                        break;
                    case "price":
                        query = searchDTO.SortOrder == "asc" ? query.OrderBy(p => p.SalePrice) : query.OrderByDescending(p => p.SalePrice);
                        break;
                    case "rating":
                        query = searchDTO.SortOrder == "asc" ? query.OrderByDescending(p => p.TotalItemsSold) : query.OrderBy(p => p.TotalItemsSold);
                        break;
                    case "date":
                        query = searchDTO.SortOrder == "asc" ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt);
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
                .Skip((searchDTO.PageNumber - 1) * searchDTO.PageSize)
                .Take(searchDTO.PageSize)
                .ToListAsync();

            return new PagedResult<Product>
            {
                Items = items,
                TotalCount = totalItems,
                PageSize = searchDTO.PageSize,
                CurrentPage = searchDTO.PageNumber
            };
        }

        public async Task<List<Product>> GetProductsByTagTypeAsync(TagType type)
        {
            if (type == TagType.BestSeller)
            {
                return await _context.Products
                .Where(p => p.ProductTags.Any(pt => pt.Tag.Type == type) && p.Status == 1)
                .Include(c => c.Category)
                .Include(c => c.Galleries.OrderBy(g => g.Order))
                .OrderByDescending(p => p.TotalItemsSold)
                .ToListAsync();
            }
            else
            {
                return await _context.Products
           .Where(p => p.ProductTags.Any(pt => pt.Tag.Type == type) && p.Status == 1)
           .Include(c => c.Category)
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
