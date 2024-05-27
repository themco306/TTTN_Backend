using backend.DTOs;
using backend.Exceptions;
using backend.Helper;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace backend.Controllers
{
        [ApiController]
        [Route("api/posts")]
        public class PostController : ControllerBase
        {
                private readonly PostService _postService;
                private readonly IHttpContextAccessor _httpContextAccessor;
                public PostController(PostService postService, IHttpContextAccessor httpContextAccessor)
                {
                        _postService = postService;
                        _httpContextAccessor = httpContextAccessor;
                }

                [HttpGet]
                [Authorize(Policy =$"{AppRole.SuperAdmin}{ClaimType.PostClaim}{ClaimValue.Show}")] 

                public async Task<IActionResult> GetPosts()
                {
                        var posts = await _postService.GetPostsAsync();
                        return Ok(posts);
                }
                [HttpGet("page")]
                [Authorize(Policy =$"{AppRole.SuperAdmin}{ClaimType.PostClaim}{ClaimValue.Show}")] 
                 
                public async Task<IActionResult> GetPostsPage()
                {
                        var posts = await _postService.GetPostsPageAsync();
                        return Ok(posts);
                }
                [HttpGet("filter")]
                public async Task<IActionResult> GetFilteredProducts([FromQuery] PostFilterDTO filter)
                {
                        var pagedResult = await _postService.GetFilteredPostsAsync(filter);
                        return Ok(pagedResult);
                }
                [HttpGet("active")]
                public async Task<IActionResult> GetPostsActive()
                {
                        var posts = await _postService.GetPostsActiveAsync();
                        return Ok(posts);
                }
                [HttpGet("{postId}")]
                public async Task<IActionResult> GetPostById(long postId)
                {
                        var post = await _postService.GetPostByIdAsync(postId);
                        return Ok(post);
                }
                [HttpGet("slug/{slug}")]
                public async Task<IActionResult> GetPostBySlug(string slug)
                {
                        var post = await _postService.GetPostBySlugAsync(slug);
                        return Ok(post);
                }
                [HttpGet("same/{slug}")]
                public async Task<IActionResult> GetPostSame(string slug)
                {
                        var post = await _postService.GetPostSameAsync(slug);
                        return Ok(post);
                }
                [HttpGet("{postId}/show")]
                public async Task<IActionResult> GetPostShowById(long postId)
                {
                        var post = await _postService.GetPostShowByIdAsync(postId);
                        return Ok(post);
                }
                [HttpPost]
                [Authorize(Policy = $"{AppRole.SuperAdmin}{ClaimType.PostClaim}{ClaimValue.Add}")]
                public async Task<IActionResult> CreateTypePost([FromForm] PostInputDTO postInputDTO)
                {
                        string tokenWithBearer = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
                        var createdPost = await _postService.CreateTypePostAsync(postInputDTO, tokenWithBearer);
                        return Ok(new { message = "Thêm bài viết thành công", data = createdPost });
                }
                [HttpPost("page")]
                [Authorize(Policy = $"{AppRole.SuperAdmin}{ClaimType.PostClaim}{ClaimValue.Add}")]
                public async Task<IActionResult> CreateTypePage([FromForm] PostInputDTO postInputDTO)
                {
                        string tokenWithBearer = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
                        var createdPost = await _postService.CreateTypePageAsync(postInputDTO, tokenWithBearer);
                        return Ok(new { message = "Thêm trang đơn thành công", data = createdPost });

                }
                [HttpPut("{postId}")]
                [Authorize(Policy = $"{AppRole.SuperAdmin}{ClaimType.PostClaim}{ClaimValue.Edit}")]

                public async Task<IActionResult> UpdatePost(long postId, [FromForm] PostInputDTO postInputDTO)
                {
                        string tokenWithBearer = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
                        var post = await _postService.UpdatePostAsync(postId, postInputDTO, tokenWithBearer);
                        return Ok(new { message = post.Type == PostType.post ? "Sửa bài viết thành công" : "Sửa trang đơn thành công", data = post });
                }

                [HttpDelete("{postId}")]
                [Authorize(Policy = $"{AppRole.SuperAdmin}{ClaimType.PostClaim}{ClaimValue.Delete}")]

                public async Task<IActionResult> DeletePost(long postId)
                {
                        await _postService.DeletePostAsync(postId);
                        return Ok(new { message = "Xóa thành công hình ảnh có ID: " + postId });

                }
                [Authorize(Policy = $"{AppRole.SuperAdmin}{ClaimType.PostClaim}{ClaimValue.Delete}")]

                [HttpDelete("delete-multiple")]
                public async Task<IActionResult> DeleteMultipleCategories(LongIDsModel model)
                {
                        if (model.ids == null || model.ids.Count == 0)
                        {
                                return BadRequest(new { error = "Danh sách các ID không được trống." });
                        }

                        await _postService.DeletePostsAsync(model.ids);
                        string concatenatedIds = string.Join(", ", model.ids);
                        return Ok(new { message = "Xóa thành công hình ảnh có ID: " + concatenatedIds });


                }
                [HttpPut("{id}/status")]
                [Authorize(Policy = $"{AppRole.SuperAdmin}{ClaimType.PostClaim}{ClaimValue.Edit}")]

                public async Task<IActionResult> UpdatePostStatus(long id)
                {

                        await _postService.UpdatePostStatusAsync(id);
                        return Ok(new { mesaage = "Thay đổi trạng thái thành công" });
                }
        }
}
