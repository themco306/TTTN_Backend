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
          private readonly IHttpContextAccessor _httpContextAccessor;
        public OrderInfoController(OrderInfoService orderinfoService,IHttpContextAccessor httpContextAccessor)
        {
            _orderinfoService = orderinfoService;
            _httpContextAccessor=httpContextAccessor;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetOrderInfos()
        {
            string tokenWithBearer = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            var orderinfos = await _orderinfoService.GetAllAsync(tokenWithBearer);
            return Ok(orderinfos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderInfoById(long id)
        {
                var orderinfo = await _orderinfoService.GetByIdAsync(id);
                return Ok(orderinfo);
        }
        [HttpPost]
                [Authorize]

        public async Task<IActionResult> CreateOrderInfo(OrderInfoInputDTO orderInfo)
        {
         string tokenWithBearer = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
                var createdOrderInfo = await _orderinfoService.CreateAsync(orderInfo,tokenWithBearer);
                 return Ok(new {message="Thêm thông tin thành công",data=createdOrderInfo});
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateOrderInfo(long id,OrderInfoInputDTO orderInfo)
        {
            string tokenWithBearer = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
                var orderinfo=await _orderinfoService.UpdateAsync(id, orderInfo,tokenWithBearer);
                return Ok(new {message="Sửa thông tin thành công",data=orderinfo});
        }

        [HttpDelete("{id}")]
         [Authorize] 
        public async Task<IActionResult> DeleteOrderInfo(long id)
        {       
                await _orderinfoService.DeleteOrderInfoAsync(id);
                return Ok(new{message="Xóa thành công thông tin."});
            
        }
    }
}
