using backend.DTOs;
using backend.Exceptions;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/carts")]
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartController(CartService cartService, IHttpContextAccessor httpContextAccessor)
        {
            _cartService = cartService;
            _httpContextAccessor=httpContextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> GetCarts()
        {
            var carts = await _cartService.GetAllCartsAsync();
            return Ok(carts);
        }

        [HttpGet("my-cart")]
        [Authorize]
        public async Task<IActionResult> GetCart()
        {
             string tokenWithBearer = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
                var cart = await _cartService.GetCartByUserIdAsync(tokenWithBearer);
                return Ok(cart);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostCart(CartInputDTO cartInput)
        {
                        string tokenWithBearer = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];

                var cart = await _cartService.CreateCartAsync(cartInput,tokenWithBearer);
                return Ok(new {message="Thêm giỏ hàng thành công",data=cart});
        }
         [HttpPut("quantity/{id}/{quantity}")]
        [Authorize]
        public async Task<IActionResult> ChangeQuantity(long id, int quantity)
        {
                        string tokenWithBearer = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];

                await _cartService.ChangeQuantityAsync(id,quantity,tokenWithBearer);
                return Ok();
        }
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCart(long id)
        {
             string tokenWithBearer = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
                await _cartService.DeleteCartAsync(id,tokenWithBearer);
                return Ok(new{message="Xóa khỏi giỏ hàng thành công"});
           
        }
    }
}
