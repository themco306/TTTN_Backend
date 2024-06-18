
using backend.DTOs;
using backend.Exceptions;
using backend.Helper;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
        [ApiController]
        [Route("api/rates")]

        public class RateController : ControllerBase
        {
                private readonly RateService _rateService;
                  private readonly IHttpContextAccessor _httpContextAccessor;
                public RateController(RateService rateService,IHttpContextAccessor httpContextAccessor)
                {
                        _rateService = rateService;
                        _httpContextAccessor=httpContextAccessor;
                }
                [HttpGet("{id}")]
                public async Task<IActionResult> GetRates([FromQuery] RateQueryDTO queryDTO,long id)
                {
                        var rates = await _rateService.GetRatesByProductIddsync(id,queryDTO);
                        return Ok(rates);
                }
                [HttpGet("active")]
                public async Task<IActionResult> GetRatesActive()
                {
                        var rates = await _rateService.GetAllRatesActiveAsync();
                        return Ok(rates);
                }
                [HttpPut("action/{rateId}/{isLike}")]
                [Authorize]
                public async Task<IActionResult> Action(long rateId,bool isLike)
                {
                          string tokenWithBearer = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
                        var data=await _rateService.ActionAsync(rateId,isLike,tokenWithBearer);
                        return Ok(new{like=data.likeCount,dislike=data.dislikeCount});
                }
                [HttpGet("rateLike/{rateId}")]
                [Authorize]
                public async Task<IActionResult> GetRateLike(long rateId)
                {
                          string tokenWithBearer = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
                        var data=await _rateService.GetRateLikeAsync(rateId,tokenWithBearer);
                        return Ok(new{data=data});
                }
                [HttpPost]
                 [Authorize] 
                public async Task<IActionResult> PostRate(RateInputDTO rateInputDTO)
                {       
                          string tokenWithBearer = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
                        var rate = await _rateService.CreateRateAsync(rateInputDTO,tokenWithBearer);
                        return Ok(new {message="Đánh giá thành công",data=rate});
                }
                [HttpPost("report/{id}")]
                 [Authorize] 
                public async Task<IActionResult> ReportRate(long id)
                {       
                        await _rateService.ReportRateAsync(id);
                        return Ok(new {message="Báo cáo thành công"});
                }
                // [HttpPut("{id}")]
                // [Authorize] 
                // public async Task<IActionResult> PutRate(long id, RateInputDTO rateInputDTO)
                // {
                //           string tokenWithBearer = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
                //         var rate = await _rateService.UpdateRateAsync(id, rateInputDTO,tokenWithBearer);
                //        return Ok(new {message="Cập nhật thương hiệu thành công",data=rate});
                // }

                [HttpDelete("{id}")]
               [Authorize] 
                public async Task<IActionResult> DeleteRate(long id)
                {
                        await _rateService.DeleteRateAsync(id);
                        return Ok(new{message="Xóa thành công danh mục có ID: " + id});
                }
                [HttpDelete("delete-multiple")]
                [Authorize] 
                public async Task<IActionResult> DeleteMultipleRates(LongIDsModel model)
                {
                        if (model.ids == null || model.ids.Count == 0)
                        {
                                return BadRequest(new{error="Danh sách các ID không được trống."});
                        }

                        await _rateService.DeleteRatesAsync(model.ids);
                        string concatenatedIds = string.Join(", ", model.ids);
                        return Ok(new{message="Xóa thành công danh mục có ID: "+concatenatedIds});


                }
                  [HttpPut("{id}/status")]
                  [Authorize(Roles =$"{AppRole.Admin},{AppRole.SuperAdmin}")] 
        public async Task<IActionResult> UpdateRateStatus(long id)
        {
           
                await _rateService.UpdateRateStatusAsync(id);
                return Ok(new {message=""});
        }



        }
}
