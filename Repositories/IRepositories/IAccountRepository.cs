using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.DTOs;
using backend.Models;
using Microsoft.AspNetCore.Identity;

namespace backend.Repositories.IRepositories
{
  public interface IAccountRepository
  {
    Task<bool> AddRolesToUserAsync(AppUser user, List<string> roles);
    Task<bool> UpdateUserRolesAsync(string userId, List<string> roles);
Task<bool> DeleteClaimsAsync(string userId);
    Task<List<string?>> GetAllRolesAsync();
    Task<AppUser> SignUpUserAsync(SignUp signUp);
    Task<AppUser> CreateUserAsync(UserCreateDTO userCreateDTO);
    Task<bool> AddRoleToUserAsync(AppUser user, string role);
    Task<AppUser> GetUserByEmailAsync(string email);
    Task<AppUser> GetUserByUserNameAsync(string userName);
    Task<bool> CheckPasswordAsync(AppUser user, string password);
    Task<SignInResult> PasswordSignInAsync(string email, string password);
    Task<string> GenerateJwtToken(AppUser user,bool rememberMe);
    Task<string> GenerateEmailConfirmationTokenAsync(AppUser user);


    Task<IEnumerable<AppUser>> GetUsersAsync(int pageIndex, int pageSize,string email);


    Task<AppUser> GetUserByIdAsync(string id);

    Task<IEnumerable<string>> GetUserRolesAsync(string userId);
    Task<List<ClaimDTO>> GetUserClaimsAsync(string id);

    Task<bool> DeleteUserByIdAsync(string userId);
    Task<bool> ResetPasswordAsync(AppUser user, string resetToken, string newPassword);
    Task<string> GeneratePasswordResetTokenAsync(AppUser user);

    Task<bool> UpdateUserAsync(string id, AppUser user);

    Task<bool> ConfirmEmailAsync(AppUser userId, string confirmEmailToken);

    Task<bool> CheckEmailConfirmedAsync(AppUser user);
    Task<bool> AddClaimToUserAsync(string user, string claimType, List<string> claimValues);

  }
}