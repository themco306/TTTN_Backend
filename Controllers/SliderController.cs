using backend.DTOs;
using backend.Exceptions;
using backend.Helper;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/sliders")]
    public class SliderController : ControllerBase
    {
        private readonly SliderService _sliderService;

        public SliderController(SliderService sliderService)
        {
            _sliderService = sliderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSliders()
        {
            var sliders = await _sliderService.GetSlidersAsync();
            return Ok(sliders);
        }
      [HttpGet("active")]
        public async Task<IActionResult> GetSlidersActive()
        {
            var sliders = await _sliderService.GetSlidersActiveAsync();
            return Ok(sliders);
        }
        [HttpGet("{sliderId}")]
        public async Task<IActionResult> GetSliderById(long sliderId)
        {
                var slider = await _sliderService.GetSliderByIdAsync(sliderId);
                return Ok(slider);
        }
        [HttpGet("{sliderId}/show")]
        public async Task<IActionResult> GetSliderShowById(long sliderId)
        {
                var slider = await _sliderService.GetSliderShowByIdAsync(sliderId);
                return Ok(slider);
        }
        [HttpPost]
         [Authorize(Policy =$"{AppRole.SuperAdmin}{ClaimType.SliderClaim}{ClaimValue.Add}")] 
        public async Task<IActionResult> CreateSlider([FromForm] SliderInputDTO sliderInputDTO)
        {
        
                var createdSlider = await _sliderService.CreateSliderAsync(sliderInputDTO);
                return CreatedAtAction(nameof(GetSliderById), new { sliderId = createdSlider.Id }, new {message="Thêm hình ảnh thành công",data=createdSlider});
        }

        [HttpPut("{sliderId}")]
         [Authorize(Policy =$"{AppRole.SuperAdmin}{ClaimType.SliderClaim}{ClaimValue.Edit}")] 

        public async Task<IActionResult> UpdateSlider(long sliderId, [FromForm] SliderInputDTO sliderInputDTO)
        {
                var slider=await _sliderService.UpdateSliderAsync(sliderId, sliderInputDTO);
                return Ok(new {message="Sửa hình ảnh thành công",data=slider});
        }

        [HttpDelete("{sliderId}")]
         [Authorize(Policy =$"{AppRole.SuperAdmin}{ClaimType.SliderClaim}{ClaimValue.Delete}")] 

        public async Task<IActionResult> DeleteSlider(long sliderId)
        {
                await _sliderService.DeleteSliderAsync(sliderId);
                return Ok(new{message="Xóa thành công hình ảnh có ID: " + sliderId});
            
        }
         [Authorize(Policy =$"{AppRole.SuperAdmin}{ClaimType.SliderClaim}{ClaimValue.Delete}")] 

        [HttpDelete("delete-multiple")]
                public async Task<IActionResult> DeleteMultipleCategories(LongIDsModel model)
                {
                        if (model.ids == null || model.ids.Count == 0)
                        {
                                return BadRequest(new{error="Danh sách các ID không được trống."});
                        }

                        await _sliderService.DeleteSlidersAsync(model.ids);
                        string concatenatedIds = string.Join(", ", model.ids);
                        return Ok(new{message="Xóa thành công hình ảnh có ID: "+concatenatedIds});


                }
                [HttpPut("{id}/status")]
         [Authorize(Policy =$"{AppRole.SuperAdmin}{ClaimType.SliderClaim}{ClaimValue.Edit}")] 

        public async Task<IActionResult> UpdateSliderStatus(long id)
        {
           
                await _sliderService.UpdateSliderStatusAsync(id);
                return Ok(new{mesaage="Thay đổi trạng thái thành công"});
        }
    }
}
