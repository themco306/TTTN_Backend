// using backend.Context;
// using backend.Models;
// using backend.Repositories.IRepositories;
// using Microsoft.EntityFrameworkCore;
// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;

// namespace backend.Repositories
// {
//     public class CartItemRepository : ICartItemRepository
//     {
//         private readonly AppDbContext _context;

//         public CartItemRepository(AppDbContext context)
//         {
//             _context = context;
//         }

//         public async Task AddAsync(CartItem cartItem)
//         {
//             _context.CartItems.Add(cartItem);
//             await _context.SaveChangesAsync();
//         }
//         public async Task UpdateAsync(CartItem cartItem)
// {
//     _context.Entry(cartItem).State = EntityState.Modified;
//     await _context.SaveChangesAsync();
// }

//         public async Task DeleteAsync(long id)
//         {
//             var cartItem = await _context.CartItems.FindAsync(id);
//             if (cartItem != null)
//             {
//                 _context.CartItems.Remove(cartItem);
//                 await _context.SaveChangesAsync();
//             }
//         }

//         public async Task<List<CartItem>> GetAllAsync()
//         {
//             return await _context.CartItems.ToListAsync();
//         }

//         public async Task<CartItem> GetByIdAsync(long id)
//         {
//             return await _context.CartItems.FindAsync(id);
//         }
//     }
// }
