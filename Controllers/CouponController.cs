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
        [HttpGet("showAll/{id}")]
        public async Task<IActionResult> GetCouponShowAll(long id)
        {
            var coupon = await _couponService.GetCouponShowAllByIdAsync(id);
            return Ok(coupon);
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
             return Ok(new{message="Cập nhật mã giảm giá "+coupon.Code+" thàng công",data=coupon});
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = $"{AppRole.SuperAdmin}{ClaimType.CouponClaim}{ClaimValue.Delete}")]
        public async Task<IActionResult> DeleteCoupon(long id)
        {
            await _couponService.DeleteCouponAsync(id);
            return Ok(new { message = "Xóa thành công mã giảm giá có STT: " + id });
        }
        [HttpDelete("delete-multiple")]
        [Authorize(Policy =$"{AppRole.SuperAdmin}{ClaimType.CouponClaim}{ClaimValue.Delete}")] 
        public async Task<IActionResult> DeleteMultipleUsers(LongIDsModel iDsModel)
        {
            if (iDsModel.ids == null || iDsModel.ids.Count == 0)
            {
                return BadRequest(new { error = "Danh sách không được trống." });
            }
            await _couponService.DeleteCouponsById(iDsModel.ids);
            string concatenatedIds = string.Join(", ", iDsModel.ids);
            return Ok(new{message="Xóa thành công mã giảm có STT: "+concatenatedIds});
        }
        [HttpPut("{id}/status")]
        [Authorize(Policy = $"{AppRole.SuperAdmin}{ClaimType.CouponClaim}{ClaimValue.Edit}")]
        public async Task<IActionResult> UpdateCouponStatus(long id)
        {
            try
            {
                await _couponService.UpdateStatusAsync(id);
                return Ok();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
