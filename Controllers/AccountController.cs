
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


        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
   
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(SignUp signUp)
        {
            var result = await _accountService.SignUpAsync(signUp);
            if (!result.Succeeded)
            {
                throw new BadRequestException("Có lỗi xảy ra, bạn nên thử lại.");
            }
            return Ok("Đăng ký thành công");
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

        [HttpGet("confirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string confirmEmailToken)
        {   
            await _accountService.ConfirmEmailAsync(userId,confirmEmailToken);
            return NoContent();
        }
        [HttpGet]
        public async Task<IActionResult> GetUsers(int pageIndex=1,int pageSize=5){
                var users= await _accountService.GetUsersAsync(pageIndex, pageSize);
                return Ok(users);
        }

         [HttpGet("{id}")]
         public async Task<IActionResult> GetUserById(string id){
            var user = await _accountService.GetUserByIdAsync(id);
            return Ok(user);
         }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id,UserCustomerUpdateDTO userCustomerUpdateDTO)
        {
                
                await _accountService.UpdateUserAsync(id,userCustomerUpdateDTO);
                return Ok("Cập nhật thành công");
        }
        [HttpDelete("{id}")]
        [Authorize(Roles =AppRole.Admin)]
        public async Task<IActionResult> DeleteUserById(string id)
        {
                await _accountService.DeleteUserById(id);
                return Ok("Xóa thành công");
        }
    }
}
