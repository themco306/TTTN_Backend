
using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using backend.Payment.Momo.Request;
using backend.Payment.Momo.Response;
using backend.Payment.Momo;
using Microsoft.AspNetCore.Authorization;


namespace backend.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly PaymentMomoService _paymentMomoService;

        public PaymentController(IHttpContextAccessor httpContextAccessor,PaymentMomoService paymentMomoService)
        {
            _httpContextAccessor=httpContextAccessor;
            _paymentMomoService=paymentMomoService;
        }

        private static readonly HttpClient client = new HttpClient();

        [HttpPost("momo")]
        [Authorize]
        public async Task<IActionResult> CreateMoMoPayment(CreateMoMoPaymentDTO moMoPaymentDTO)
        {
              string tokenWithBearer = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            var data=await _paymentMomoService.GetLinkPayment(moMoPaymentDTO,tokenWithBearer);
            return Ok(data.payUrl);
        }

        [HttpPost("momo/ipn")]
        public async Task<IActionResult> MomoIpn([FromBody] MoMoSuccessDTO response)
        {
           var success= await _paymentMomoService.PaymentSuccess(response);
           return Ok(new{message="Thanh toán thành công"});
        }

    }
}
