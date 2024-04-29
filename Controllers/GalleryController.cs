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
        [HttpPost("{productId}")]
        public async Task<IActionResult> AddImagesToProduct(long productId, [FromForm] List<IFormFile> images)
        {
            try
            {
                await _galleryService.CreateGalleryAsync(productId, images);
                return Ok(new { message ="Images added successfully."});
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

        [HttpPut("{id}")]
        public async Task<IActionResult> PutGallery(long id,[FromForm] LongIDsModel model,[FromForm] List<IFormFile> images)
        {
            List<long> ids = model.ids ?? new List<long>();
    await _galleryService.UpdateGalleryImagesAsync(id, ids, images);
    return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGallery(long id)
        {
            await _galleryService.DeleteGalleryAsync(id);
            return NoContent();
        }
    }
}
