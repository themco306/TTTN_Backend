using backend.Exceptions;
using backend.Models;
using backend.Repositories.IRepositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Services
{
    public class CartItemService
    {
        private readonly ICartItemRepository _cartItemRepository;

        public CartItemService(ICartItemRepository cartItemRepository)
        {
            _cartItemRepository = cartItemRepository;
        }

        public async Task<List<CartItem>> GetAllCartItemsAsync()
        {
            return await _cartItemRepository.GetAllAsync();
        }

        public async Task<CartItem> GetCartItemByIdAsync(long id)
        {
            var cartItem = await _cartItemRepository.GetByIdAsync(id);
            if (cartItem == null)
            {
                throw new NotFoundException("CartItem not found.");
            }
            return cartItem;
        }

        public async Task AddCartItemAsync(CartItem cartItem)
        {
            await _cartItemRepository.AddAsync(cartItem);
        }

        public async Task DeleteCartItemAsync(long id)
        {
            await _cartItemRepository.DeleteAsync(id);
        }
    }
}
