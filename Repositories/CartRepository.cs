using backend.Context;
using backend.Models;
using backend.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Cart cart)
        {
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart != null)
            {
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Cart>> GetAllAsync()
        {
            return await _context.Carts.ToListAsync();
        }

        public async Task<Cart> GetByIdAsync(long id)
        {
            return await _context.Carts.FirstOrDefaultAsync(c=>c.Id==id);
        }
        public async Task<Cart> GetByUserIdAsync(string id)
        {
            return await _context.Carts.Include(c=>c.CartItems).ThenInclude(ci=>ci.Product).ThenInclude(cii=>cii.Galleries).FirstOrDefaultAsync(c=>c.UserId==id);
        }
    }
}
