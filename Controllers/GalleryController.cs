using backend.DTOs;
using backend.Exceptions;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/galleries")]
    public class GalleryController : ControllerBase
    {
        private readonly GalleryService _galleryService;

        public GalleryController(GalleryService galleryService)
        {
            _galleryService = galleryService;
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetGalleriesByProductId(long productId)
        {
            var galleries = await _galleryService.GetGalleriesByProductIdAsync(productId);
            return Ok(galleries);
        }

        // [HttpPost]
        // public async Task<IActionResult> PostGallery([FromForm] GalleryInputDTO galleryInputDTO)
        // {
        //     var galleries = await _galleryService.CreateGalleryAsync(galleryInputDTO);
        //     return Ok(galleries);
        // }
        [HttpPost]
        public async Task<IActionResult> AddImagesToProduct(long productId, [FromForm] List<IFormFile> images)
        {
            try
            {
                await _galleryService.UpdateGalleryImagesAsync(productId, images);
                return Ok("Images added successfully.");
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // [HttpPut("{id}")]
        // public async Task<IActionResult> PutGallery(long id, GalleryInputDTO galleryInputDTO)
        // {
        //     await _galleryService.UpdateGalleryAsync(id, galleryInputDTO);
        //     return NoContent();
        // }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGallery(long id)
        {
            await _galleryService.DeleteGalleryAsync(id);
            return NoContent();
        }
    }
}
