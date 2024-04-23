// using backend.Exceptions;
// using backend.Models;
// using backend.Repositories.IRepositories;
// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;

// namespace backend.Services
// {
//     public class CartService
//     {
//         private readonly ICartRepository _cartRepository;

//         public CartService(ICartRepository cartRepository)
//         {
//             _cartRepository = cartRepository;
//         }

//         public async Task<List<Cart>> GetAllCartsAsync()
//         {
//             return await _cartRepository.GetAllAsync();
//         }

//         public async Task<Cart> GetCartByIdAsync(long id)
//         {
//             var cart = await _cartRepository.GetByIdAsync(id);
//             if (cart == null)
//             {
//                 throw new NotFoundException("Giỏ hàng không tồn tại.");
//             }
//             return cart;
//         }

//         public async Task<Cart> CreateCartAsync(string userId)
//         {
//             // Tạo một giỏ hàng mới
//             var cart = new Cart
//             {
//                 UserId = userId // Gán người dùng cho giỏ hàng mới
//             };

//             await _cartRepository.AddAsync(cart);
//             return cart;
//         }

//         public async Task DeleteCartAsync(long id)
//         {
//             var cart = await _cartRepository.GetByIdAsync(id);
//             if (cart == null)
//             {
//                 throw new NotFoundException("Giỏ hàng không tồn tại.");
//             }
//             await _cartRepository.DeleteAsync(id);
//         }

//         // Các phương thức khác cần thiết cho quản lý giỏ hàng có thể được thêm vào ở đây

//     }
// }
