using backend.DTOs;
using backend.Exceptions;
using backend.Helper;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderInfos()
        {
            var orderinfos = await _orderService.GetAllAsync();
            return Ok(orderinfos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderInfoById(long id)
        {
                var order = await _orderService.GetByIdAsync(id);
                return Ok(order);
        }
        [HttpPost]
        //  [Authorize(Policy =$"{AppRole.SuperAdmin}{ClaimType.OrderInfoClaim}{ClaimValue.Add}")] 
        public async Task<IActionResult> CreateOrderInfo(OrderInputDTO order)
        {
        
                var createdOrderInfo = await _orderService.CreateAsync(order);
                return CreatedAtAction(nameof(GetOrderInfoById), new { id = createdOrderInfo.Id }, new {message="Thêm thông tin giao hàng thành công",data=createdOrderInfo});
        }

        // [HttpPut("{id}")]
        // //  [Authorize(Policy =$"{AppRole.SuperAdmin}{ClaimType.OrderInfoClaim}{ClaimValue.Edit}")] 

        // public async Task<IActionResult> UpdateOrderInfo(long id,Order order)
        // {
        //         var order=await _orderService.UpdateAsync(id, order);
        //         return Ok(new {message="Sửa hình ảnh thành công",data=order});
        // }

        // [HttpDelete("{id}")]
        // //  [Authorize] 
        // public async Task<IActionResult> DeleteOrderInfo(long id)
        // {
        //         await _orderService.DeleteOrderInfoAsync(id);
        //         return Ok(new{message="Xóa thành công thông tin."});
            
        // }
    }
}
