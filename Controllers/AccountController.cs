
using backend.DTOs;
using backend.Exceptions;
using backend.Helper;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Google.Apis.Auth;

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
         [HttpPost("google-login")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
    {
        var payload = await VerifyGoogleToken(request.Credential,request.ClientId);
        if (payload == null)
        {
            return BadRequest(new { message = "Invalid Google token." });
        }
        var user = await _accountService.GenerateJwtTokenForGoogle(payload);

        return Ok(new { User = user.User, Token = user.Token });
    }
 private async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(string token,string clientId)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string>() { clientId }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);
            return payload;
        }
        catch (InvalidJwtException)
        {
            return null;
        }
    }
        [HttpGet("newUser")]
        [Authorize]
        public async Task<IActionResult> GetNewUserDashboard(){
            var newUser=await _accountService.GetNewCustomer();
            return Ok(new {data=newUser});
        }
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(SignUp signUp)
        {
            var result = await _accountService.SignUpAsync(signUp);
            if (!result.Succeeded)
            {
                throw new BadRequestException("Có lỗi xảy ra, bạn nên thử lại.");
            }
            return Ok(new { message = "Đăng ký thành công" });
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
        [HttpPost("signinAdmin")]
        public async Task<IActionResult> SignAdminIn(SignIn signIn)
        {
            var signInResult = await _accountService.SignInAdminAsync(signIn);
            if (signInResult == null)
            {
                throw new BadRequestException("Có lỗi xảy ra, bạn nên thử lại.");
            }

            return Ok(new { User = signInResult.User, Token = signInResult.Token });
        }
        [HttpPost("sendEmailConfirm/{id}")]
        public async Task<IActionResult> SendEmailConfirm(string id, [FromBody] string url)
        {
            await _accountService.SendEmailConfirm(id, url);
            return Ok(new { message = "Gửi liên kết thành công vui lòng vào Email để xác nhận" });
        }
        [HttpPost("sendResetPasswordConfirm")]
        public async Task<IActionResult> SendResetPasswordConfirm([FromBody] ResetPasswordInputDTO inputDTO)
        {
            await _accountService.SendRestPasswordConfirm(inputDTO);
            return Ok(new { message = "Gửi liên kết thành công vui lòng vào Email để xác nhận" });
        }
        [HttpPost("confirmEmail/{id}")]
        public async Task<IActionResult> ConfirmEmail(string id, [FromBody] string confirmEmailToken)
        {
            await _accountService.ConfirmEmailAsync(id, confirmEmailToken);
            return Ok(new { message = "Xác nhận email thành công" });
        }
        [HttpPost("confirmSetPassword")]
        public async Task<IActionResult> ConfirmSetPassword([FromBody] SetPasswordInputDTO inputDTO)
        {
            await _accountService.ConfirmSetPasswordAsync(inputDTO);
            return Ok(new { message = "Tạo mật khẩu mới thành công" });
        }
        [HttpGet]
        [Authorize(Policy = $"{AppRole.SuperAdmin}{ClaimType.UserClaim}{ClaimValue.Show}")]
        public async Task<IActionResult> GetUsers([FromQuery] ProductAdminFilterDTO filterDTO)
        {
            string tokenWithBearer = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            var userEmail = _accountService.ExtractEmailFromToken(tokenWithBearer);
            if (userEmail == null)
            {
                return Unauthorized(new { error = "Có lỗi xãy ra vui lòng đăng nhập lại" });
            }
            var users = await _accountService.GetUsersAsync(filterDTO, userEmail);
            return Ok(users);
        }
        [HttpGet("customer")]
        [Authorize(Policy = $"{AppRole.SuperAdmin}{ClaimType.UserClaim}{ClaimValue.Show}")]
        public async Task<IActionResult> GetCustomes([FromQuery] ProductAdminFilterDTO filterDTO)
        {
            string tokenWithBearer = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            var userEmail = _accountService.ExtractEmailFromToken(tokenWithBearer);
            if (userEmail == null)
            {
                return Unauthorized(new { error = "Có lỗi xãy ra vui lòng đăng nhập lại" });
            }
            var users = await _accountService.GetCusomerAsync(filterDTO, userEmail);
            return Ok(users);
        }
        [HttpGet("roles")]
        [Authorize]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _accountService.GetRolesWithTempIdsAsync();
            return Ok(roles);
        }
        [HttpGet("myEdit")]
        [Authorize]
        public async Task<IActionResult> GetMyUser()
        {
            string tokenWithBearer = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            var user = await _accountService.GetMyUserAsync(tokenWithBearer);
            return Ok(user);

        }
        [HttpGet("{id}")]
        [Authorize(Policy = $"{AppRole.SuperAdmin}{ClaimType.UserClaim}{ClaimValue.Show}")]
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
        public async Task<IActionResult> UpdateUser(string id, [FromForm] UserUpdateByAdminDTO updateByAdminDTO, IFormFile? avatar)
        {

            await _accountService.UpdateUserByAdminAsync(id, updateByAdminDTO, avatar);
            return Ok(new { message = "Cập nhật thành công" });
        }
        [HttpPut("my/{id}")]
        [Authorize]


        public async Task<IActionResult> UpdateMyUser(string id, [FromForm] MyUserUpdateDTO userUpdateDTO, IFormFile? avatar)
        {
            string tokenWithBearer = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            var userEmail = _accountService.ExtractEmailFromToken(tokenWithBearer);
            if (userEmail == null)
            {
                return Unauthorized(new { error = "Có lỗi xãy ra vui lòng đăng nhập lại" });
            }
            var checkMatches = await _accountService.CheckUserIdMatchesEmail(id, userEmail);
            if (!checkMatches)
            {
                return BadRequest(new { error = "Bạn không thể sửa người khác" });
            }

            var user = await _accountService.UpdateMyUserAsync(id, userUpdateDTO, avatar);
            return Ok(new { message = "Cập nhật thành công", data = user });

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
                return Unauthorized(new { error = "Có lỗi xãy ra vui lòng đăng nhập lại" });
            }
            var checkMatches = await _accountService.CheckUserIdMatchesEmail(id, userEmail);
            if (checkMatches)
            {
                return BadRequest(new { error = "Bạn không thể xóa chính mình" });
            }
            await _accountService.DeleteUserById(id);
            return Ok(new { message = "Xóa thành công" });
        }
        [HttpDelete("delete-multiple")]
        [Authorize(Policy = $"{AppRole.SuperAdmin}{ClaimType.UserClaim}{ClaimValue.Delete}")]
        public async Task<IActionResult> DeleteMultipleUsers(StringIDsModel iDsModel)
        {
            string tokenWithBearer = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            var userEmail = _accountService.ExtractEmailFromToken(tokenWithBearer);
            if (userEmail == null)
            {
                return Unauthorized(new { error = "Có lỗi xãy ra vui lòng đăng nhập lại" });
            }
            foreach (var id in iDsModel.ids)
            {
                var checkMatches = await _accountService.CheckUserIdMatchesEmail(id, userEmail);
                if (checkMatches)
                {
                    return BadRequest(new { error = "Bạn không thể xóa chính mình" });
                }
            }
            await _accountService.DeleteUsersById(iDsModel.ids);
            return Ok(new { message = "Xóa thành công" });
        }
    }
}
