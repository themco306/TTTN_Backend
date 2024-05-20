using backend.DTOs;
using backend.Helper;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;



namespace backend.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderController(OrderService orderService, IHttpContextAccessor httpContextAccessor)
        {
            _orderService = orderService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderInfos()
        {
            var orderinfos = await _orderService.GetAllAsync();
            return Ok(orderinfos);
        }
        [HttpGet("myOrder")]
        [Authorize]
        public async Task<IActionResult> GetMyOrders(int page=1,int pageSize=5)
        {
              string tokenWithBearer = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            var orderinfos = await _orderService.GetMyOrdersAsync(tokenWithBearer,page,pageSize);
            return Ok(orderinfos);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderInfoById(long id)
        {
                var order = await _orderService.GetByIdAsync(id);
                return Ok(order);
        }
        [HttpGet("code/{code}")]
        [Authorize]
        public async Task<IActionResult> GetOrderByCode(string code)
        {
                  string tokenWithBearer = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
                var order = await _orderService.GetByCodeAsync(code,tokenWithBearer);
                return Ok(order);
        }
        [HttpPost("sendEmailConfirm/{id}")]
        [Authorize]
        public async Task<IActionResult> SendEmailConfirmOrder(long id,[FromBody] string confirmationUrl  )
        {
             string tokenWithBearer = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
           await _orderService.SendEmailConfirm(id,confirmationUrl,tokenWithBearer);
           return Ok(new{message="Gửi liên kết thành công vui lòng vào Email để xác nhận"});
        }
        [HttpPost("confirmEmail/{id}")]
        [Authorize]
        public async Task<IActionResult> ConfirmEmail(string id,[FromBody] string confirmEmailToken)
        {
            await _orderService.ConfirmEmailAsync(id, confirmEmailToken);
            return Ok(new{message="Xác nhận đặt hàng thành công"});
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateOrderInfo(OrderInputDTO order)
        {
                 string tokenWithBearer = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
                var createdOrderInfo = await _orderService.CreateAsync(order,tokenWithBearer);
                return Ok(new {message="Đặt hàng thành công",data=createdOrderInfo});
        }

        [HttpPut("{id}")]
         [Authorize(Policy =$"{AppRole.SuperAdmin}{ClaimType.ProductClaim}{ClaimValue.Edit}")] 

        public async Task<IActionResult> UpdateOrderStatus(long id,OrderUpdateStatusDTO statusDTO)
        {
                await _orderService.UpdateStatusAsync(id, statusDTO.Status);
                return Ok(new {message="Thay đổi trạng thái thành công"});
        }

 
    }
}
