
using backend.DTOs;
using backend.Exceptions;
using backend.Helper;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace backend.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly AccountService _accountService;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public AccountController(AccountService accountService, IHttpContextAccessor httpContextAccessor)
        {
            _accountService = accountService;
            _httpContextAccessor = httpContextAccessor;

        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(SignUp signUp)
        {
            var result = await _accountService.SignUpAsync(signUp);
            if (!result.Succeeded)
            {
                throw new BadRequestException("Có lỗi xảy ra, bạn nên thử lại.");
            }
            return Ok(new { message ="Đăng ký thành công"});
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn(SignIn signIn)
        {
            var signInResult = await _accountService.SignInAsync(signIn);
            if (signInResult == null)
            {
                throw new BadRequestException("Có lỗi xảy ra, bạn nên thử lại.");
            }

            return Ok(new { User = signInResult.User, Token = signInResult.Token });
        }
        [HttpPost("sendEmailConfirm/{id}")]
        public async Task<IActionResult> SendEmailConfirm(string id,[FromBody] string url  )
        {
           await _accountService.SendEmailConfirm(id,url);
           return Ok(new{message="Gửi liên kết thành công vui lòng vào Email để xác nhận"});
        }
        [HttpPost("confirmEmail/{id}")]
        public async Task<IActionResult> ConfirmEmail(string id,[FromBody] string confirmEmailToken)
        {
            await _accountService.ConfirmEmailAsync(id, confirmEmailToken);
            return Ok(new{message="Xác nhận email thành công"});
        }
        [HttpGet]
        public async Task<IActionResult> GetUsers(int pageIndex = 1, int pageSize = 5)
        {
            var users = await _accountService.GetUsersAsync(pageIndex, pageSize);
            return Ok(users);
        }
                [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _accountService.GetRolesWithTempIdsAsync();
            return Ok(roles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _accountService.GetUserByIdAsync(id);
            return Ok(user);

        }
        [HttpPost]
        [Authorize(Policy = $"{AppRole.SuperAdmin}{ClaimType.UserClaim}{ClaimValue.Add}")]
        public async Task<IActionResult> PostUser(UserCreateDTO userCreateDTO)
        {
            var result = await _accountService.CreateUserAsync(userCreateDTO);
            if (result == null)
            {
                throw new BadRequestException("Có lỗi xảy ra, bạn nên thử lại.");
            }
            return Ok(new { message = "Tạo Người dùng thành công", data = result });
        }
        [HttpPut("{id}")]
        [Authorize(Policy = $"{AppRole.SuperAdmin}{ClaimType.UserClaim}{ClaimValue.Edit}")]
        public async Task<IActionResult> UpdateUser(string id,[FromForm] UserUpdateByAdminDTO updateByAdminDTO,IFormFile? avatar)
        {

            await _accountService.UpdateUserByAdminAsync(id, updateByAdminDTO,avatar);
            return Ok(new { message ="Cập nhật thành công"});
        }
        [HttpPut("my/{id}")]
        [Authorize(Roles = AppRole.SuperAdmin)]
        [Authorize(Roles = AppRole.Admin)]
        public async Task<IActionResult> UpdateMyUser(string id,[FromForm] MyUserUpdateDTO userUpdateDTO,IFormFile? avatar)
        {
                        string tokenWithBearer = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            var userEmail = _accountService.ExtractEmailFromToken(tokenWithBearer);
            if (userEmail == null)
            {
                return Unauthorized(new { error ="Có lỗi xãy ra vui lòng đăng nhập lại"});
            }
            var checkMatches = await _accountService.CheckUserIdMatchesEmail(id, userEmail);
            if (!checkMatches)
            {
                return BadRequest(new { error ="Bạn không thể sửa người khác"});
            }

            await _accountService.UpdateMyUserAsync(id, userUpdateDTO,avatar);
            return Ok(new { message ="Cập nhật thành công"});

        }
        [HttpPut("statusEmail/{id}")]
        [Authorize(Policy = $"{AppRole.SuperAdmin}{ClaimType.UserClaim}{ClaimValue.Edit}")]
        public async Task<IActionResult> ChangeStatusEmailUser(string id)
        {

            await _accountService.ChangeStatusUserAsync(id);
            return Ok("Cập nhật thành công");
        }
        [HttpDelete("{id}")]
        [Authorize(Policy = $"{AppRole.SuperAdmin}{ClaimType.UserClaim}{ClaimValue.Delete}")]
        public async Task<IActionResult> DeleteUserById(string id)
        {
            string tokenWithBearer = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            var userEmail = _accountService.ExtractEmailFromToken(tokenWithBearer);
            if (userEmail == null)
            {
                return Unauthorized(new { error ="Có lỗi xãy ra vui lòng đăng nhập lại"});
            }
            var checkMatches = await _accountService.CheckUserIdMatchesEmail(id, userEmail);
            if (checkMatches)
            {
                return BadRequest(new { error ="Bạn không thể xóa chính mình"});
            }
            await _accountService.DeleteUserById(id);
            return Ok(new { message ="Xóa thành công"});
        }
        [HttpDelete("delete-multiple")]
        [Authorize(Policy =$"{AppRole.SuperAdmin}{ClaimType.UserClaim}{ClaimValue.Delete}")] 
        public async Task<IActionResult> DeleteMultipleUsers(StringIDsModel iDsModel)
        {
            string tokenWithBearer = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            var userEmail = _accountService.ExtractEmailFromToken(tokenWithBearer);
            if (userEmail == null)
            {
                return Unauthorized(new { error ="Có lỗi xãy ra vui lòng đăng nhập lại"});
            }
            foreach (var id in iDsModel.ids)
            {
            var checkMatches = await _accountService.CheckUserIdMatchesEmail(id, userEmail);
            if (checkMatches)
            {
                return BadRequest(new { error ="Bạn không thể xóa chính mình"});
            }
            }
            await _accountService.DeleteUsersById(iDsModel.ids);
            return Ok(new { message ="Xóa thành công"});
        }
    }
}
