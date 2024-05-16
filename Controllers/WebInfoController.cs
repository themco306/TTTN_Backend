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
    [Route("api/web-infos")]
    public class WebInfoController : ControllerBase
    {
        private readonly WebInfoService _webinfoService;

        public WebInfoController(WebInfoService webinfoService)
        {
            _webinfoService = webinfoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetWebInfos()
        {
            var webinfos = await _webinfoService.GetFirstWebInfoAsync();
            return Ok(webinfos);
        }
        [HttpPut("{webinfoId}")]
        public async Task<IActionResult> UpdateWebInfo(long webinfoId, [FromForm] WebInfoInputDTO webinfoInputDTO)
        {
                var webinfo=await _webinfoService.UpdateWebInfoAsync(webinfoId, webinfoInputDTO);
                return Ok(new {message="Thông tin website thành công",data=webinfo});
        }
    }
}
