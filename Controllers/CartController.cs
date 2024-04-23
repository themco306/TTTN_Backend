// using backend.Exceptions;
// using backend.Services;
// using Microsoft.AspNetCore.Mvc;
// using System;
// using System.Threading.Tasks;

// namespace backend.Controllers
// {
//     [ApiController]
//     [Route("api/carts")]
//     public class CartController : ControllerBase
//     {
//         private readonly CartService _cartService;

//         public CartController(CartService cartService)
//         {
//             _cartService = cartService;
//         }

//         [HttpGet]
//         public async Task<IActionResult> GetCarts()
//         {
//             var carts = await _cartService.GetAllCartsAsync();
//             return Ok(carts);
//         }

//         [HttpGet("{id}")]
//         public async Task<IActionResult> GetCart(long id)
//         {
//             try
//             {
//                 var cart = await _cartService.GetCartByIdAsync(id);
//                 return Ok(cart);
//             }
//             catch (NotFoundException ex)
//             {
//                 return NotFound(ex.Message);
//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(500, ex.Message);
//             }
//         }

//         // [HttpPost]
//         // public async Task<IActionResult> PostCart(string userId)
//         // {
//         //     try
//         //     {
//         //         var cart = await _cartService.CreateCartAsync(userId);
//         //         return CreatedAtAction("GetCart", new { id = cart.Id }, cart);
//         //     }
//         //     catch (Exception ex)
//         //     {
//         //         return StatusCode(500, ex.Message);
//         //     }
//         // }

//         [HttpDelete("{id}")]
//         public async Task<IActionResult> DeleteCart(long id)
//         {
//             try
//             {
//                 await _cartService.DeleteCartAsync(id);
//                 return NoContent();
//             }
//             catch (NotFoundException ex)
//             {
//                 return NotFound(ex.Message);
//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(500, ex.Message);
//             }
//         }

//         // Các phương thức khác cần thiết cho quản lý giỏ hàng có thể được thêm vào ở đây
//     }
// }
