
using backend.DTOs;
using backend.Exceptions;
using backend.Helper;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
        [ApiController]
        [Route("api/brands")]

        public class BrandController : ControllerBase
        {
                private readonly BrandService _brandService;
                  private readonly IHttpContextAccessor _httpContextAccessor;
                public BrandController(BrandService brandService,IHttpContextAccessor httpContextAccessor)
                {
                        _brandService = brandService;
                        _httpContextAccessor=httpContextAccessor;
                }
                [HttpGet]
                public async Task<IActionResult> GetBrands()
                {
                        var brands = await _brandService.GetAllBrandsAsync();
                        return Ok(brands);
                }
                                [HttpGet("active")]
                public async Task<IActionResult> GetBrandsActive()
                {
                        var brands = await _brandService.GetAllBrandsActiveAsync();
                        return Ok(brands);
                }
                [HttpGet("{id}")]
                public async Task<IActionResult> GetBrand(long id)
                {
                        var brand = await _brandService.GetBrandByIdAsync(id);
                        return Ok(brand);
                }

                [HttpPost]

                 [Authorize(Policy =$"{AppRole.SuperAdmin}{ClaimType.BrandClaim}{ClaimValue.Add}")] 
                public async Task<IActionResult> PostBrand(BrandInputDTO brandInputDTO)
                {       
                          string tokenWithBearer = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
                        var brand = await _brandService.CreateBrandAsync(brandInputDTO,tokenWithBearer);
                        return Ok(new {message="Thêm thương hiệu thành công",data=brand});
                }

                [HttpPut("{id}")]
                [Authorize(Policy =$"{AppRole.SuperAdmin}{ClaimType.BrandClaim}{ClaimValue.Edit}")] 

                public async Task<IActionResult> PutBrand(long id, BrandInputDTO brandInputDTO)
                {
                          string tokenWithBearer = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
                        var brand = await _brandService.UpdateBrandAsync(id, brandInputDTO,tokenWithBearer);
                       return Ok(new {message="Cập nhật thương hiệu thành công",data=brand});
                }

                [HttpDelete("{id}")]
               [Authorize(Policy =$"{AppRole.SuperAdmin}{ClaimType.BrandClaim}{ClaimValue.Delete}")] 
                public async Task<IActionResult> DeleteBrand(long id)
                {
                        await _brandService.DeleteBrandAsync(id);
                        return Ok(new{message="Xóa thành công danh mục có ID: " + id});
                }
                [HttpDelete("delete-multiple")]
                [Authorize(Policy =$"{AppRole.SuperAdmin}{ClaimType.BrandClaim}{ClaimValue.Delete}")] 
                public async Task<IActionResult> DeleteMultipleBrands(LongIDsModel model)
                {
                        if (model.ids == null || model.ids.Count == 0)
                        {
                                return BadRequest(new{error="Danh sách các ID không được trống."});
                        }

                        await _brandService.DeleteBrandsAsync(model.ids);
                        string concatenatedIds = string.Join(", ", model.ids);
                        return Ok(new{message="Xóa thành công danh mục có ID: "+concatenatedIds});


                }
                  [HttpPut("{id}/status")]
                  [Authorize(Policy =$"{AppRole.SuperAdmin}{ClaimType.BrandClaim}{ClaimValue.Edit}")] 
        public async Task<IActionResult> UpdateBrandStatus(long id)
        {
           
                await _brandService.UpdateBrandStatusAsync(id);
                return Ok(new {message="Thay đổi trạng thái thành công"});
        }



        }
}
