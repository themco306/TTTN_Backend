using backend.DTOs;
using backend.Exceptions;
using backend.Models;
using backend.Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Services
{
    public class CartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly AccountService _accountService;
        private readonly ProductService _productService;

        public CartService(ICartRepository cartRepository,ICartItemRepository cartItemRepository,AccountService accountService,ProductService productService)
        {
            _cartRepository = cartRepository;
            _cartItemRepository=cartItemRepository;
            _accountService=accountService;
            _productService=productService;
        }

        public async Task<List<Cart>> GetAllCartsAsync()
        {
            return await _cartRepository.GetAllAsync();
        }

        public async Task<List<CartItem>> GetCartByUserIdAsync(string token)
        {
            var userId = _accountService.ExtractUserIdFromToken(token);
            if (userId == null)
            {
                throw new NotFoundException("Có lỗi xãy ra vui lòng đăng nhập lại");
            }
            var cart = await _cartRepository.GetByUserIdAsync(userId);
            if (cart == null)
            {
                throw new NotFoundException("Giỏ hàng không tồn tại.");
            }
            return cart.CartItems;
        }

        public async Task<CartItem> CreateCartAsync(CartInputDTO cartInput,string token)
        {
            var userId = _accountService.ExtractUserIdFromToken(token);
            if (userId == null)
            {
                throw new NotFoundException("Có lỗi xãy ra vui lòng đăng nhập lại");
            }
            var user = await _accountService.GetUserByIdAsync(userId);
            var cart = await _cartRepository.GetByUserIdAsync(user.Id);
            if(cart==null){
                var createdCart=new Cart{
                    UserId=user.Id,
                };
                await _cartRepository.AddAsync(createdCart);
                cart=createdCart;
            }
            var product= await _productService.GetProductByIdAsync(cartInput.CartItem.ProductId);

            var exitCartItem = await _cartItemRepository.ExitingdAsync(cart.Id,cartInput.CartItem.ProductId);
            if(exitCartItem==null){
                if(product.Quantity<1){
                    throw new NotFoundException("Số lượng sản phẩm trong kho không đủ");
                }
                 var cartItem =new CartItem
            {
                CartId=cart.Id,
                ProductId=cartInput.CartItem.ProductId,
                Quantity=cartInput.CartItem.Quantity
            };
            await _cartItemRepository.AddAsync(cartItem);
            exitCartItem=cartItem;
            }else{
                if(product.Quantity<=exitCartItem.Quantity){
                    throw new NotFoundException("Số lượng sản phẩm trong kho không đủ");
                }
                exitCartItem.Quantity+=cartInput.CartItem.Quantity;
                await _cartItemRepository.UpdateAsync(exitCartItem);
            }

           
            
            return exitCartItem;
        }
        public async Task ChangeQuantityAsync(long id ,int quantity,string token){
             var userId = _accountService.ExtractUserIdFromToken(token);
            if (userId == null)
            {
                throw new NotFoundException("Có lỗi xãy ra vui lòng đăng nhập lại");
            }
            var user = await _accountService.GetUserByIdAsync(userId);
            var cartItem=await _cartItemRepository.GetByIdAsync(id);
            var cart =await _cartRepository.GetByIdAsync(cartItem.CartId);
            if(user.Id!=cart.UserId){
                throw new NotFoundException("Có lỗi xãy ra vui lòng đăng nhập lại");
            }
            var product=await _productService.GetProductByIdAsync(cartItem.ProductId);
            if(quantity<=0||quantity>product.Quantity){
                    throw new NotFoundException("Số lượng không phù hợp");
            }
            cartItem.Quantity=quantity;
            await _cartItemRepository.UpdateAsync(cartItem);
        }

        public async Task DeleteCartAsync(long id,string token)
        {
                try
                {
                    var userId = _accountService.ExtractUserIdFromToken(token);
            if (userId == null)
            {
                throw new NotFoundException("Có lỗi xãy ra vui lòng đăng nhập lại");
            }
            var user = await _accountService.GetUserByIdAsync(userId);
            await _cartItemRepository.DeleteAsync(id);
                }
                catch (System.Exception)
                {
                    
                    throw new Exception("Lỗi hệ thống vui lòng tải lại trang");
                }
              
        }

        // Các phương thức khác cần thiết cho quản lý giỏ hàng có thể được thêm vào ở đây

    }
}
