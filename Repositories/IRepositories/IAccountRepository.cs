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
    Task<AppUser> CreateUserAsync(SignUp signUp);
    Task<bool> AddRoleToUserAsync(AppUser user, string role);
    Task<AppUser> GetUserByEmailAsync(string email);
    Task<bool> CheckPasswordAsync(AppUser user, string password);
    Task<SignInResult> PasswordSignInAsync(string email, string password);
    Task<string> GenerateJwtToken(AppUser user);
    Task<string> GenerateEmailConfirmationTokenAsync(AppUser user);

    Task<IEnumerable<AppUser>> GetUsersAsync(int pageIndex, int pageSize);

    Task<AppUser> GetUserByIdAsync(string id);

    Task<IEnumerable<string>> GetUserRolesAsync(string userId);

    Task<bool> DeleteUserByIdAsync(string userId);

    Task<bool> UpdateUserAsync(string id, AppUser user);

    Task<bool> ConfirmEmailAsync(AppUser userId, string confirmEmailToken);

    Task<bool> CheckEmailConfirmedAsync(AppUser user);

  }
}