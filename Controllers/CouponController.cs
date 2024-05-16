using backend.DTOs;
using backend.Exceptions;
using backend.Helper;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/coupons")]
    public class CouponController : ControllerBase
    {
        private readonly CouponService _couponService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CouponController(CouponService couponService, IHttpContextAccessor httpContextAccessor)
        {
            _couponService = couponService;
            _httpContextAccessor = httpContextAccessor;
            
        }

        [HttpGet]
        public async Task<IActionResult> GetCoupons()
        {
            var coupons = await _couponService.GetAllCouponsAsync();
            return Ok(coupons);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCoupon(long id)
        {
            var coupon = await _couponService.GetCouponByIdAsync(id);
            return Ok(coupon);
        }
        [HttpGet("code/{code}")]
        public async Task<IActionResult> SubmitCode(string code)
        {
            var coupon = await _couponService.GetCouponByCodeAsync(code);
            return Ok(coupon);
        }

        [HttpPost]
        [Authorize(Policy = $"{AppRole.SuperAdmin}{ClaimType.CouponClaim}{ClaimValue.Add}")]
        public async Task<IActionResult> PostCoupon(CouponInputDTO couponInputDTO)
        {
            string tokenWithBearer = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            var coupon = await _couponService.CreateCouponAsync(couponInputDTO,tokenWithBearer);
            return Ok(new{message="Thêm mã giảm giá thàng công",data=coupon});
        }

        [HttpPut("{id}")]
        [Authorize(Policy = $"{AppRole.SuperAdmin}{ClaimType.CouponClaim}{ClaimValue.Edit}")]
        public async Task<IActionResult> PutCoupon(long id, CouponInputDTO couponInputDTO)
        {
            var coupon = await _couponService.UpdateCouponAsync(id, couponInputDTO);
            return CreatedAtAction("GetCoupon", new { id }, new { message = "Sửa thành công mã giảm giá: " + coupon.Code, data = coupon });
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = $"{AppRole.SuperAdmin}{ClaimType.CouponClaim}{ClaimValue.Delete}")]
        public async Task<IActionResult> DeleteCoupon(long id)
        {
            await _couponService.DeleteCouponAsync(id);
            return Ok(new { message = "Xóa thành công mã giảm giá có ID: " + id });
        }
    }
}
