using AutoMapper;
using backend.DTOs;
using backend.Exceptions;
using backend.Helper;
using backend.Models;
using backend.Repositories.IRepositories;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(long id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            return Ok(product);
        }

        [HttpPost]
        // [Authorize(Roles = AppRole.Admin)]
        public async Task<IActionResult> PostProduct(ProductInputDTO productInputDTO)
        {
            var product = await _productService.CreateProductAsync(productInputDTO);
            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = AppRole.Admin)]
        public async Task<IActionResult> PutProduct(long id, ProductInputDTO productInputDTO)
        {
            var product = await _productService.UpdateProductAsync(id, productInputDTO);
             return CreatedAtAction("GetProduct", new { id }, product);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = AppRole.Admin)]
        public async Task<IActionResult> DeleteProduct(long id)
        {
            await _productService.DeleteProductAsync(id);
            return NoContent();
        }
         [HttpDelete("delete-multiple")]
                public async Task<IActionResult> DeleteMultipleCategories(IDsModel model)
                {
                        if (model.ids == null || model.ids.Count == 0)
                        {
                                return BadRequest("Danh sách các ID không được trống.");
                        }

                        await _productService.DeleteProductsAsync(model.ids);
                        return Ok("Xóa danh mục thành công.");


                }

                                  [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateProductStatus(long id)
        {
            try
            {
                var product =await _productService.UpdateProductStatusAsync(id);
                return Ok(product);
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
