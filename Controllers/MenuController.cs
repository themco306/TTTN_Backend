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
    [Route("api/menus")]
    public class MenuController : ControllerBase
    {
        private readonly MenuService _menuService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public MenuController(MenuService menuService,IHttpContextAccessor httpContextAccessor)
        {
            _menuService = menuService;
            _httpContextAccessor=httpContextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> GetMenus()
        {
            var sliders = await _menuService.GetMenusAsync();
            return Ok(sliders);
        }
      [HttpGet("header")]
        public async Task<IActionResult> GetMenuHeader()
        {
            var sliders = await _menuService.GetMenusHeaderAsync();
            return Ok(sliders);
        }
              [HttpGet("footer")]
        public async Task<IActionResult> GetMenuFooter()
        {
            var sliders = await _menuService.GetMenusFooterAsync();
            return Ok(sliders);
        }
              [HttpGet("sub/{id}")]
        public async Task<IActionResult> GetSubMenu(long id)
        {
            var sliders = await _menuService.GetSubMenusAsync(id);
            return Ok(sliders);
        }
        [HttpGet("{sliderId}")]
        public async Task<IActionResult> GetById(long sliderId)
        {
                var slider = await _menuService.GetMenuByIdAsync(sliderId);
                return Ok(slider);
        }
                        [HttpGet("parent/{id}")]
                public async Task<IActionResult> GetParents(long id)
                {
                        var categories = await _menuService.GetParentsAsync(id);
                        return Ok(categories);
                }
//         [HttpGet("{sliderId}/show")]
//         public async Task<IActionResult> GetSliderShowById(long sliderId)
//         {
//                 var slider = await _sliderService.GetSliderShowByIdAsync(sliderId);
//                 return Ok(slider);
//         }
        [HttpPost("custom")]
         [Authorize(Policy =$"{AppRole.SuperAdmin}{ClaimType.SliderClaim}{ClaimValue.Add}")] 
        public async Task<IActionResult> CreateMenuCustom(MenuCustomInputDTO menuCustom)
        {
                string tokenWithBearer = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
                var menu = await _menuService.CreateCustomMenuAsync(menuCustom,tokenWithBearer);
                return Ok(new {message="Thêm Menu thành công",data=menu});
        }       
        [HttpPost]
         [Authorize(Policy =$"{AppRole.SuperAdmin}{ClaimType.SliderClaim}{ClaimValue.Add}")] 
        public async Task<IActionResult> CreateMenu(MenuInputDTO menuDTO)
        {
                string tokenWithBearer = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
                var menu = await _menuService.CreateMenuAsync(menuDTO,tokenWithBearer);
                return Ok(new {message="Thêm Menu thành công",data=menu});
        }     
        [HttpPut("{id}")]
         [Authorize(Policy =$"{AppRole.SuperAdmin}{ClaimType.SliderClaim}{ClaimValue.Edit}")] 

        public async Task<IActionResult> UpdateSlider(long id, MenuInputUpdateDTO menuInputUpdate)
        {
                 string tokenWithBearer = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
                var menu=await _menuService.UpdateMenuAsync(id, menuInputUpdate,tokenWithBearer);
                return Ok(new {message="Sửa Menu thành công",data=menu});
        }

        [HttpDelete("{sliderId}")]
         [Authorize(Policy =$"{AppRole.SuperAdmin}{ClaimType.SliderClaim}{ClaimValue.Delete}")] 

        public async Task<IActionResult> DeleteSlider(long sliderId)
        {
                await _menuService.DeleteMenuAsync(sliderId);
                return Ok(new{message="Xóa thành công menu có ID: " + sliderId});
            
        }
         [Authorize(Policy =$"{AppRole.SuperAdmin}{ClaimType.SliderClaim}{ClaimValue.Delete}")] 

        [HttpDelete("delete-multiple")]
                public async Task<IActionResult> DeleteMultipleCategories(LongIDsModel model)
                {
                        if (model.ids == null || model.ids.Count == 0)
                        {
                                return BadRequest(new{error="Danh sách các ID không được trống."});
                        }

                        await _menuService.DeleteMenusAsync(model.ids);
                        string concatenatedIds = string.Join(", ", model.ids);
                        return Ok(new{message="Xóa thành công menu có ID: "+concatenatedIds});


                }
                [HttpPut("{id}/status")]
         [Authorize(Policy =$"{AppRole.SuperAdmin}{ClaimType.SliderClaim}{ClaimValue.Edit}")] 

        public async Task<IActionResult> UpdateMenuStatus(long id)
        {
           
                await _menuService.UpdateMenuStatusAsync(id);
                return Ok(new{mesaage="Thay đổi trạng thái thành công"});
        }
    }
}
