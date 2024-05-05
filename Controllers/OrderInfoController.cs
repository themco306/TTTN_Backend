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
    [Route("api/orderInfos")]
    public class OrderInfoController : ControllerBase
    {
        private readonly OrderInfoService _orderinfoService;

        public OrderInfoController(OrderInfoService orderinfoService)
        {
            _orderinfoService = orderinfoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderInfos()
        {
            var orderinfos = await _orderinfoService.GetAllAsync();
            return Ok(orderinfos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderInfoById(long id)
        {
                var orderinfo = await _orderinfoService.GetByIdAsync(id);
                return Ok(orderinfo);
        }
        [HttpPost]
        //  [Authorize(Policy =$"{AppRole.SuperAdmin}{ClaimType.OrderInfoClaim}{ClaimValue.Add}")] 
        public async Task<IActionResult> CreateOrderInfo(OrderInfo orderInfo)
        {
        
                var createdOrderInfo = await _orderinfoService.CreateAsync(orderInfo);
                return CreatedAtAction(nameof(GetOrderInfoById), new { id = createdOrderInfo.Id }, new {message="Thêm thông tin giao hàng thành công",data=createdOrderInfo});
        }

        [HttpPut("{id}")]
        //  [Authorize(Policy =$"{AppRole.SuperAdmin}{ClaimType.OrderInfoClaim}{ClaimValue.Edit}")] 

        public async Task<IActionResult> UpdateOrderInfo(long id,OrderInfo orderInfo)
        {
                var orderinfo=await _orderinfoService.UpdateAsync(id, orderInfo);
                return Ok(new {message="Sửa hình ảnh thành công",data=orderinfo});
        }

        [HttpDelete("{id}")]
        //  [Authorize] 
        public async Task<IActionResult> DeleteOrderInfo(long id)
        {
                await _orderinfoService.DeleteOrderInfoAsync(id);
                return Ok(new{message="Xóa thành công thông tin."});
            
        }
    }
}
