

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend.DTOs;
using backend.Helper;
using backend.Models;
using backend.Repositories.IRepositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace backend.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AccountRepository(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IConfiguration configuration, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _roleManager = roleManager;
        }

        public async Task<AppUser> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }
        public async Task<AppUser> GetUserByUserNameAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public async Task<bool> CheckPasswordAsync(AppUser user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }
        public async Task<bool> CheckEmailConfirmedAsync(AppUser user)
        {
            return await _userManager.IsEmailConfirmedAsync(user);
        }
        public async Task<SignInResult> PasswordSignInAsync(string email, string password)
        {
            return await _signInManager.PasswordSignInAsync(email, password, false, false);
        }

        public async Task<string> GenerateJwtToken(AppUser user)
        {
            var authClaim = new List<Claim>{
        new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Email, user.Email),
        new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                authClaim.Add(new Claim(ClaimTypes.Role, role.ToString()));
            }
            var userClaims = await _userManager.GetClaimsAsync(user);
            foreach (var claim in userClaims)
            {
                authClaim.Add(new Claim(claim.Type, claim.Value));
            }
            var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:ValidIssuer"],
                audience: _configuration["Jwt:ValidAudience"],
                claims: authClaim,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha256Signature)
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<AppUser> SignUpUserAsync(SignUp signUp)
        {
            var user = new AppUser
            {
                FirstName = signUp.FirstName,
                LastName = signUp.LastName,
                Email = signUp.Email,
                UserName = signUp.Email.Split('@')[0]
            };
            var result = await _userManager.CreateAsync(user, signUp.Password);
            if (!result.Succeeded)
            {
                return null;
            }
            return user;
        }
        public async Task<AppUser> CreateUserAsync(UserCreateDTO userCreateDTO)
        {
            var user = new AppUser
            {
                FirstName = userCreateDTO.FirstName,
                LastName = userCreateDTO.LastName,
                Email = userCreateDTO.Email,
                PhoneNumber = userCreateDTO.PhoneNumber,
                Gender = userCreateDTO.Gender,
                Avatar = userCreateDTO.Gender == true ? "avatar-nam.jpg" : "avatar-nu.jpg",
                UserName = userCreateDTO.Email.Split('@')[0]
            };
            var result = await _userManager.CreateAsync(user, userCreateDTO.Password);
            if (!result.Succeeded)
            {
                return null;
            }
            return user;
        }
        public async Task<bool> AddRoleToUserAsync(AppUser user, string role)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
            var result = await _userManager.AddToRoleAsync(user, role);
            return result.Succeeded;


        }
        public async Task<bool> AddRolesToUserAsync(AppUser user, List<string> roles)
        {
            bool success = true;

            foreach (var role in roles)
            {
                if (await _roleManager.RoleExistsAsync(role))
                {
                    var result = await _userManager.AddToRoleAsync(user, role);
                    if (!result.Succeeded)
                    {
                        success = false;
                    }
                }
            }

            return success;
        }

        public async Task<bool> UpdateUserRolesAsync(string userId, List<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            // Xóa các vai trò hiện tại của người dùng
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            // Thêm các vai trò mới cho người dùng
            var result = await _userManager.AddToRolesAsync(user, roles);

            return result.Succeeded;
        }
        public async Task<bool> DeleteClaimsAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            // Xóa các claim hiện tại của người dùng
            var currentClaims = await _userManager.GetClaimsAsync(user);
            foreach (var claim in currentClaims)
            {
                await _userManager.RemoveClaimAsync(user, claim);
            }
            return true;
        }
        public async Task<bool> AddClaimToUserAsync(string userId, string claimType, List<string> claimValues)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }
            // Kiểm tra xem claim đã tồn tại chưa, nếu không thì tạo mới
            foreach (var claimValue in claimValues)
            {
                var existingClaims = await _userManager.GetClaimsAsync(user);
                if (!existingClaims.Any(c => c.Type == claimType && c.Value == claimValue))
                {
                    var newClaim = new Claim(claimType, claimValue);
                    await _userManager.AddClaimAsync(user, newClaim);
                }
            }


            return true;
        }


        public async Task<string> GenerateEmailConfirmationTokenAsync(AppUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            return token;
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return null;
            }

            return await _userManager.GetRolesAsync(user);
        }
        public async Task<List<ClaimDTO>> GetUserClaimsAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return null;
            }

            var userClaims = await _userManager.GetClaimsAsync(user);

            // Dictionary để theo dõi các giá trị claim theo loại claim
            var claimDictionary = new Dictionary<string, List<string>>();

            foreach (var claim in userClaims)
            {
                if (!claimDictionary.ContainsKey(claim.Type))
                {
                    claimDictionary.Add(claim.Type, new List<string>());
                }
                claimDictionary[claim.Type].Add(claim.Value);
            }

            // Chuyển đổi từ Dictionary sang danh sách ClaimDTO
            var claimDTOs = claimDictionary.Select(pair => new ClaimDTO
            {
                ClaimType = pair.Key,
                ClaimValues = pair.Value
            }).ToList();

            return claimDTOs;
        }
        public async Task<IEnumerable<AppUser>> GetUsersAsync(int pageIndex, int pageSize)
        {
            return await _userManager.Users
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return null;
            }
            return user;
        }
        public async Task<string> GeneratePasswordResetTokenAsync(AppUser user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }
        public async Task<bool> ResetPasswordAsync(AppUser user, string resetToken, string newPassword)
        {
            var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
            return result.Succeeded;
        }
        public async Task<bool> UpdateUserAsync(string id, AppUser user)
        {


            // Thực hiện cập nhật thông tin người dùng
            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }
        public async Task<bool> DeleteUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return false;
            }

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> ConfirmEmailAsync(AppUser user, string confirmEmailToken)
        {
            // Xác nhận email
            var result = await _userManager.ConfirmEmailAsync(user, confirmEmailToken);
            if (result.Succeeded)
            {
                return true; // Xác nhận thành công
            }
            else
            {
                return false; // Xác nhận không thành công
            }
        }
        public async Task<List<string?>> GetAllRolesAsync()
        {
            return await _roleManager.Roles.Select(r => r.Name).ToListAsync();
        }


    }
}