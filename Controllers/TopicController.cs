using backend.DTOs;
using backend.Exceptions;
using backend.Helper;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/topics")]
    public class TopicController : ControllerBase
    {
        private readonly TopicService _topicService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TopicController(TopicService topicService, IHttpContextAccessor httpContextAccessor)
        {
            _topicService = topicService;
            _httpContextAccessor = httpContextAccessor;

        }

        [HttpGet]
        public async Task<IActionResult> GetTopics()
        {
            var topics = await _topicService.GetAllTopicsAsync();
            return Ok(topics);
        }
        [HttpGet("parent/{id}")]
                public async Task<IActionResult> GetParentCategories(long id)
                {
                        var categories = await _topicService.GetParentCategoriesAsync(id);
                        return Ok(categories);
                }
        [HttpGet("showAll/{id}")]
        public async Task<IActionResult> GetTopicShowAll(long id)
        {
            var topic = await _topicService.GetTopicShowAllByIdAsync(id);
            return Ok(topic);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTopic(long id)
        {
            var topic = await _topicService.GetTopicByIdAsync(id);
            return Ok(topic);
        }
        [HttpGet("child/{id}")]
        public async Task<IActionResult> GetChildByParentId(long id)
        {
            var topic = await _topicService.GetChildByParentIdAsync(id);
            return Ok(topic);
        }
        [HttpPost]
        [Authorize(Policy = $"{AppRole.SuperAdmin}{ClaimType.CouponClaim}{ClaimValue.Add}")]
        public async Task<IActionResult> PostTopic(TopicInputDTO topicInputDTO)
        {
            string tokenWithBearer = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            var topic = await _topicService.CreateTopicAsync(topicInputDTO, tokenWithBearer);
            return Ok(new { message = "Thêm chủ đề thàng công", data = topic });
        }

        [HttpPut("{id}")]
        [Authorize(Policy = $"{AppRole.SuperAdmin}{ClaimType.CouponClaim}{ClaimValue.Edit}")]
        public async Task<IActionResult> PutTopic(long id, TopicUpdateDTO topicInputDTO)
        {
            string tokenWithBearer = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            var topic = await _topicService.UpdateTopicAsync(id, topicInputDTO, tokenWithBearer);
            return Ok(new { message = "Cập nhật chủ đề  " + topic.Name + " thàng công", data = topic });
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = $"{AppRole.SuperAdmin}{ClaimType.CouponClaim}{ClaimValue.Delete}")]
        public async Task<IActionResult> DeleteTopic(long id)
        {
            await _topicService.DeleteTopicAsync(id);
            return Ok(new { message = "Xóa thành công chủ đề có ID: " + id });
        }
        [HttpDelete("delete-multiple")]
        [Authorize(Policy = $"{AppRole.SuperAdmin}{ClaimType.CouponClaim}{ClaimValue.Delete}")]
        public async Task<IActionResult> DeleteMultipleUsers(LongIDsModel iDsModel)
        {
            if (iDsModel.ids == null || iDsModel.ids.Count == 0)
            {
                return BadRequest(new { error = "Danh sách không được trống." });
            }
            await _topicService.DeleteTopicsById(iDsModel.ids);
            string concatenatedIds = string.Join(", ", iDsModel.ids);
            return Ok(new { message = "Xóa thành công chủ đề có ID: " + concatenatedIds });
        }
        [HttpPut("{id}/status")]
        [Authorize(Policy = $"{AppRole.SuperAdmin}{ClaimType.CouponClaim}{ClaimValue.Edit}")]
        public async Task<IActionResult> UpdateTopicStatus(long id)
        {
            try
            {
                await _topicService.UpdateStatusAsync(id);
                return Ok();
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
