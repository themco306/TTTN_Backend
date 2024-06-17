

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
        private readonly Generate _generate;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AccountRepository(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IConfiguration configuration, RoleManager<IdentityRole> roleManager,Generate generate)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _generate=generate;
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

        public async Task<string> GenerateJwtToken(AppUser user,bool rememberMe)
        {
            var authClaim = new List<Claim>{
        new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Email, user.Email),
         new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.NameId, user.Id),
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
            DateTime expires;
    if (rememberMe)
    {
        // Nếu người dùng chọn "Nhớ tôi", token sẽ hết hạn sau một khoảng thời gian dài (ví dụ: 30 ngày)
        expires = DateTime.UtcNow.AddDays(30);
    }
    else
    {
        // Nếu không, token sẽ hết hạn sau một khoảng thời gian ngắn (ví dụ: 1 ngày)
        expires = DateTime.UtcNow.AddDays(1);
    }
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:ValidIssuer"],
                audience: _configuration["Jwt:ValidAudience"],
                claims: authClaim,
                expires: expires,
                signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha256Signature)
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<AppUser> SignUpUserAsync(SignUp signUp)
        {
            var now = DateTime.UtcNow;
                now = TimeZoneInfo.ConvertTimeFromUtc(now, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            var user = new AppUser
            {
                CreatedAt=now,
                UpdatedAt=now,
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
             var now = DateTime.UtcNow;
                now = TimeZoneInfo.ConvertTimeFromUtc(now, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            var user = new AppUser
            {
                CreatedAt=now,
                UpdatedAt=now,
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
        public async Task<PagedResult<AppUser>> GetUsersAsync(string roleName,ProductAdminFilterDTO filterDTO,string email)
        {
                  var role = await _roleManager.FindByNameAsync(roleName);

         if (role == null)
    {
        return new PagedResult<AppUser>
        {
            TotalCount = 0,
            PageSize = filterDTO.PageSize,
            CurrentPage = filterDTO.PageNumber,
            Items = Enumerable.Empty<AppUser>().ToList()
        };
    }
    var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);

    // Create an IQueryable from the usersInRole
    IQueryable<AppUser> query = usersInRole.AsQueryable();

    // Apply initial filtering
    query = query.Where(u => u.Email != email)
        .OrderByDescending(p => p.UpdatedAt)
     ;

            if (filterDTO.Status != null)
            {
                var Status = filterDTO.Status== 1?true:false;
                query = query.Where(c => c.EmailConfirmed == Status);
            }


if (!string.IsNullOrEmpty(filterDTO.Key))
{
    string filter = filterDTO.Key.Trim();

    if (filter.Contains("+"))
    {
        var parts = filter.Split('+', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 2)
        {
            string lastNamePart = parts[0].Trim(); // Phần trước dấu +
            string firstNamePart = parts[1].Trim(); // Phần sau dấu +

            query = query.Where(p => 
                p.LastName.Contains(firstNamePart, StringComparison.CurrentCultureIgnoreCase) &&
                p.FirstName.Contains(lastNamePart, StringComparison.CurrentCultureIgnoreCase));
        }
    }
    else
    {
       query = query.Where(p =>
    (p.LastName != null && p.LastName.Contains(filter, StringComparison.CurrentCultureIgnoreCase)) ||
    (p.FirstName != null && p.FirstName.Contains(filter, StringComparison.CurrentCultureIgnoreCase)) ||
    (p.NormalizedEmail != null && p.NormalizedEmail.Contains(filter, StringComparison.CurrentCultureIgnoreCase)) ||
    (p.PhoneNumber != null && p.PhoneNumber.Contains(filter, StringComparison.CurrentCultureIgnoreCase)));

    }
}


            if (!string.IsNullOrEmpty(filterDTO.SortOrder))
            {
                switch (filterDTO.SortOrder)
                {
                    case "create-asc":
                        query =  query.OrderByDescending(p => p.CreatedAt);
                        break;
                        case "create-desc":
                        query =  query.OrderBy(p => p.CreatedAt);
                        break;
                    default:
                        break;
                }
            }
            var totalItems =  query.Count();

            var items =  query
                .Skip((filterDTO.PageNumber - 1) * filterDTO.PageSize)
                .Take(filterDTO.PageSize)
                .ToList();

            return new PagedResult<AppUser>
            {
                Items = items,
                TotalCount = totalItems,
                PageSize = filterDTO.PageSize,
                CurrentPage = filterDTO.PageNumber
            };
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
            return await _roleManager.Roles.Where(c=>c.Name!=AppRole.SuperAdmin).Select(r => r.Name).ToListAsync();
        }
public async Task<IEnumerable<AppUser>> GetUsersInRoleAsync(string roleName)
{
    // Kiểm tra xem vai trò có tồn tại không
    if (!await _roleManager.RoleExistsAsync(roleName))
    {
        // Nếu vai trò không tồn tại, trả về danh sách rỗng
        return Enumerable.Empty<AppUser>();
    }

    // Lấy danh sách người dùng trong vai trò
    var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
    
    return usersInRole;
}
  public async Task<IEnumerable<AppUser>> GetUsersCreatedBetweenDates(DateTime startDate, DateTime endDate)
        {
            // Lấy danh sách người dùng được tạo ra trong khoảng thời gian cụ thể
            var users = await _userManager.Users
                .Where(u => u.CreatedAt >= startDate && u.CreatedAt < endDate)
                .ToListAsync();

            return users;
        }
    }
}