using backend.DTOs;
using backend.Exceptions;
using backend.Helper;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/ratefiles")]
    public class RateFileController : ControllerBase
    {
        private readonly RateFileService _rateFileService;

        public RateFileController(RateFileService rateFileService)
        {
            _rateFileService = rateFileService;
        }

        [HttpGet("{rateId}")]
        public async Task<IActionResult> GetRateFilesByRateId(long rateId)
        {
            var rateFiles = await _rateFileService.GetRateFilesByRateIdAsync(rateId);
            return Ok(rateFiles);
        }

        [HttpPost("{rateId}")]
        public async Task<IActionResult> AddFilesToRate(long rateId, [FromForm] List<IFormFile> files)
        {
            try
            {
                await _rateFileService.CreateRateFilesAsync(rateId, files);
                return Ok(new { message = "Files added successfully." });
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

        [HttpPut("{rateId}")]
        public async Task<IActionResult> UpdateRateFiles(long rateId, [FromForm] LongIDsModel model, [FromForm] List<IFormFile> files)
        {
            List<long> ids = model.ids ?? new List<long>();
            await _rateFileService.UpdateRateFilesAsync(rateId, ids, files);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRateFile(long id)
        {
            try
            {
                await _rateFileService.DeleteRateFileAsync(id);
                return NoContent();
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
    }
}
