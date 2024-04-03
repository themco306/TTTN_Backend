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
            await _productService.UpdateProductAsync(id, productInputDTO);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = AppRole.Admin)]
        public async Task<IActionResult> DeleteProduct(long id)
        {
            await _productService.DeleteProductAsync(id);
            return NoContent();
        }
    }
}
