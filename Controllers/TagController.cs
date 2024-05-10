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
    [Route("api/tags")]
    public class TagController : ControllerBase
    {
        private readonly TagService _tagService;

        public TagController(TagService tagService)
        {
            _tagService = tagService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTags()
        {
            var tags = await _tagService.GetAllTagAsync();
            return Ok(tags);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTagName(long id,[FromBody] TagNameUpdateDTO tagI){
                var tag = await _tagService.UpdateTagNameAsync(id,tagI.Name);
                return Ok(new {message="Cập nhật tên thành công",data=tag});
        }
        [HttpPut("sort")]
        public async Task<IActionResult> UpdateTagSort(LongIDsModel iDsModel){
                await _tagService.UpdateTagOrderAsync(iDsModel.ids);
                return Ok(new {message="Cập nhật vị trí thành công"});
        }

       
    }
}
