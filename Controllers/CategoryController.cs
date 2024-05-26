
using backend.DTOs;
using backend.Exceptions;
using backend.Helper;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
        [ApiController]
        [Route("api/categories")]

        public class CategoryController : ControllerBase
        {
                private readonly CategoryService _categoryService;

                public CategoryController(CategoryService categoryService)
                {
                        _categoryService = categoryService;
                }
                [HttpGet]
                public async Task<IActionResult> GetCategories()
                {
                        var categories = await _categoryService.GetAllCategoriesAsync();
                        return Ok(categories);
                }
                [HttpGet("active")]
                public async Task<IActionResult> GetCategoriesActive()
                {
                        var categories = await _categoryService.GetAllCategoriesActiveAsync();
                        return Ok(categories);
                }
                [HttpGet("parent/{id}")]
                public async Task<IActionResult> GetParentCategories(long id)
                {
                        var categories = await _categoryService.GetParentCategoriesAsync(id);
                        return Ok(categories);
                }
                [HttpGet("{id}")]


                public async Task<IActionResult> GetCategory(long id)
                {
                        var category = await _categoryService.GetCategoryByIdAsync(id);
                        return Ok(category);
                }

                [HttpPost]

                 [Authorize(Policy =$"{AppRole.SuperAdmin}{ClaimType.CategoryClaim}{ClaimValue.Add}")] 
                public async Task<IActionResult> PostCategory(CategoryInputDTO categoryInputDTO)
                {
                        var category = await _categoryService.CreateCategoryAsync(categoryInputDTO);
                        return CreatedAtAction("GetCategory", new { id = category.Id },new{message="Thêm thành công danh mục: "+category.Name,data=category} );
                }

                [HttpPut("{id}")]
                [Authorize(Policy =$"{AppRole.SuperAdmin}{ClaimType.CategoryClaim}{ClaimValue.Edit}")] 

                public async Task<IActionResult> PutCategory(long id, CategoryInputDTO categoryInputDTO)
                {
                        var category = await _categoryService.UpdateCategoryAsync(id, categoryInputDTO);
                        return CreatedAtAction("GetCategory", new { id },new{message="Sửa thành công danh mục: "+category.Name,data=category});
                }

                [HttpDelete("{id}")]
               [Authorize(Policy =$"{AppRole.SuperAdmin}{ClaimType.CategoryClaim}{ClaimValue.Delete}")] 
                public async Task<IActionResult> DeleteCategory(long id)
                {
                        await _categoryService.DeleteCategoryAsync(id);
                        return Ok(new{message="Xóa thành công danh mục có ID: " + id});
                }
                [HttpGet("child/{id}")]
                public async Task<IActionResult> GetChildByParentId(long id)
                {
                        var categories = await _categoryService.GetChildByParentIdAsync(id);
                        return Ok(categories);
                }
                [HttpDelete("delete-multiple")]
                [Authorize(Policy =$"{AppRole.SuperAdmin}{ClaimType.CategoryClaim}{ClaimValue.Delete}")] 
                public async Task<IActionResult> DeleteMultipleCategories(LongIDsModel model)
                {
                        if (model.ids == null || model.ids.Count == 0)
                        {
                                return BadRequest(new{error="Danh sách các ID không được trống."});
                        }

                        await _categoryService.DeleteCategoriesAsync(model.ids);
                        string concatenatedIds = string.Join(", ", model.ids);
                        return Ok(new{message="Xóa thành công danh mục có ID: "+concatenatedIds});


                }
                  [HttpPut("{id}/status")]
                  [Authorize(Policy =$"{AppRole.SuperAdmin}{ClaimType.CategoryClaim}{ClaimValue.Edit}")] 
        public async Task<IActionResult> UpdateCategoryStatus(long id)
        {
           
                var category =await _categoryService.UpdateCategoryStatusAsync(id);
                return Ok(category);
        }



        }
}
