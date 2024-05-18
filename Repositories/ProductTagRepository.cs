
using backend.Context;
using backend.Models;
using backend.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class ProductTagRepository : IProductTagRepository
    {
        private readonly AppDbContext _context;

        public ProductTagRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ProductTag productTag)
        {
             _context.ProductTags.Add(productTag);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(long productId, long tagId)
        {
             var productTag = await _context.ProductTags.FirstOrDefaultAsync(p=>p.ProductId==productId&&p.TagId==tagId);
            if (productTag != null)
            {
                _context.ProductTags.Remove(productTag);
                await _context.SaveChangesAsync();
            } 
        }
    }
}
