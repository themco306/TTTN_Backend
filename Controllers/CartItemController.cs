// using backend.Exceptions;
// using backend.Models;
// using backend.Services;
// using Microsoft.AspNetCore.Mvc;
// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;

// namespace backend.Controllers
// {
//     [ApiController]
//     [Route("api/cartitems")]
//     public class CartItemController : ControllerBase
//     {
//         private readonly CartItemService _cartItemService;

//         public CartItemController(CartItemService cartItemService)
//         {
//             _cartItemService = cartItemService;
//         }

//         [HttpGet]
//         public async Task<IActionResult> GetAllCartItems()
//         {
//             var cartItems = await _cartItemService.GetAllCartItemsAsync();
//             return Ok(cartItems);
//         }

//         [HttpGet("{id}")]
//         public async Task<IActionResult> GetCartItemById(long id)
//         {
//             try
//             {
//                 var cartItem = await _cartItemService.GetCartItemByIdAsync(id);
//                 return Ok(cartItem);
//             }
//             catch (NotFoundException ex)
//             {
//                 return NotFound(ex.Message);
//             }
//         }

//         [HttpPost]
//         public async Task<IActionResult> AddCartItem(CartItem cartItem)
//         {
//             try
//             {
//                 await _cartItemService.AddCartItemAsync(cartItem);
//                 return Ok("CartItem added successfully.");
//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(500, ex.Message);
//             }
//         }

//         [HttpDelete("{id}")]
//         public async Task<IActionResult> DeleteCartItem(long id)
//         {
//             try
//             {
//                 await _cartItemService.DeleteCartItemAsync(id);
//                 return Ok("CartItem deleted successfully.");
//             }
//             catch (NotFoundException ex)
//             {
//                 return NotFound(ex.Message);
//             }
//         }
//     }
// }
