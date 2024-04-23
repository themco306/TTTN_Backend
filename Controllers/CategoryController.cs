
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
                        return CreatedAtAction("GetCategory", new { id = category.Id }, category);
                }

                [HttpPut("{id}")]
                [Authorize(Policy =$"{AppRole.SuperAdmin}{ClaimType.CategoryClaim}{ClaimValue.Edit}")] 

                public async Task<IActionResult> PutCategory(long id, CategoryInputDTO categoryInputDTO)
                {
                        var category = await _categoryService.UpdateCategoryAsync(id, categoryInputDTO);
                        return CreatedAtAction("GetCategory", new { id }, category);
                }

                [HttpDelete("{id}")]
               [Authorize(Policy =$"{AppRole.SuperAdmin}{ClaimType.CategoryClaim}{ClaimValue.Delete}")] 
                public async Task<IActionResult> DeleteCategory(long id)
                {
                        await _categoryService.DeleteCategoryAsync(id);
                        return NoContent();
                }
                [HttpGet("child/{id}")]
                public async Task<IActionResult> GetChildByParentId(long id)
                {
                        var categories = await _categoryService.GetChildByParentIdAsync(id);
                        return Ok(categories);
                }
                [HttpDelete("delete-multiple")]
                [Authorize(Policy =$"{AppRole.SuperAdmin}{ClaimType.CategoryClaim}{ClaimValue.Delete}")] 
                public async Task<IActionResult> DeleteMultipleCategories(IDsModel model)
                {
                        if (model.ids == null || model.ids.Count == 0)
                        {
                                return BadRequest("Danh sách các ID không được trống.");
                        }

                        await _categoryService.DeleteCategoriesAsync(model.ids);
                        return Ok("Xóa danh mục thành công.");


                }
                  [HttpPut("{id}/status")]
                  [Authorize(Policy =$"{AppRole.SuperAdmin}{ClaimType.CategoryClaim}{ClaimValue.Edit}")] 
        public async Task<IActionResult> UpdateCategoryStatus(long id)
        {
            try
            {
                var category =await _categoryService.UpdateCategoryStatusAsync(id);
                return Ok(category);
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
