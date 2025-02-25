using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using AutoMapper;
using Azure.Core;
using backend.DTOs;
using backend.Exceptions;
using backend.Helper;
using backend.Models;
using backend.Repositories.IRepositories;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using UnauthorizedAccessException = backend.Exceptions.UnauthorizedAccessException;

namespace backend.Services
{
    public class AccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly EmailService _emailService;
        private readonly GalleryService _galleryService;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly Generate _generate;

        public AccountService(IAccountRepository accountRepository, EmailService emailService, IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor, IMapper mapper, GalleryService galleryService, UserManager<AppUser> userManager,Generate generate)
        {
            _accountRepository = accountRepository;
            _emailService = emailService;
            _urlHelperFactory = urlHelperFactory;
            _actionContextAccessor = actionContextAccessor;
            _mapper = mapper;
            _galleryService = galleryService;
            _userManager = userManager;
            _generate=generate;
        }
        public async Task<AppUser> FindOrCreateUser(GoogleJsonWebSignature.Payload payload)
        {
            var user = await _accountRepository.GetUserByEmailAsync(payload.Email);
            if (user == null)
            {
                var now = DateTime.UtcNow;
                now = TimeZoneInfo.ConvertTimeFromUtc(now, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
                var newUser = new AppUser
                {
                    CreatedAt = now,
                    UpdatedAt = now,
                    FirstName = payload.GivenName,
                    LastName = payload.FamilyName,
                    Email = payload.Email,
                    EmailConfirmed=true,
                    UserName =  payload.Email.Split('@')[0],
                    Gender = true,
                    Avatar = payload.Picture
                };
                var result = await _userManager.CreateAsync(newUser);
                if (!result.Succeeded)
                {
                    throw new BadRequestException("Thất bại vui lòng thử lại");
                }
                var resultRoles = await _accountRepository.AddRoleToUserAsync(newUser, AppRole.Customer);
                if (!resultRoles)
                {
                    throw new Exception("Quyền này không tồn tại");
                }
                var loginInfo = new UserLoginInfo("Google", payload.Subject, "Google");
                result = await _userManager.AddLoginAsync(newUser, loginInfo);
                if (!result.Succeeded)
                {
                    throw new BadRequestException("Thất bại vui lòng thử lại");

                }
                return newUser;
            }
            else
            {
                var logins = await _userManager.GetLoginsAsync(user);
                if (!logins.Any(l => l.LoginProvider == "Google" && l.ProviderKey == payload.Subject))
                {
                    var loginInfo = new UserLoginInfo("Google", payload.Subject, "Google");
                    var result = await _userManager.AddLoginAsync(user, loginInfo);
                    if (!result.Succeeded)
                    {
                        throw new BadRequestException("Thất bại vui lòng thử lại");

                    }
                }
                return user;
            }

        }
        public async Task<SignInResultDTO> GenerateJwtTokenForGoogle(GoogleJsonWebSignature.Payload payload){
            var user =await FindOrCreateUser(payload);
            var roles = await _accountRepository.GetUserRolesAsync(user.Id);
            var token = await _accountRepository.GenerateJwtToken(user, true);
            var claims = await _accountRepository.GetUserClaimsAsync(user.Id);

            var signInResult = new SignInResultDTO
            {
                User = _mapper.Map<UserGetDTO>(user),
                Token = token
            };
            signInResult.User.Roles = roles;
            signInResult.User.Claims = claims;
            return signInResult;
        }
        public async Task<GetNewCustomerDTO> GetNewCustomer()
        {
            var today = DateTime.Today;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            var firstDayOfYear = new DateTime(today.Year, 1, 1);

            // Lấy danh sách người dùng được tạo trong ngày hôm nay
            var usersCreatedToday = await _accountRepository.GetUsersCreatedBetweenDates(today, today.AddDays(1));

            // Lấy danh sách người dùng được tạo trong tháng này
            var usersCreatedThisMonth = await _accountRepository.GetUsersCreatedBetweenDates(firstDayOfMonth, today.AddDays(1));

            // Lấy danh sách người dùng được tạo trong năm nay
            var usersCreatedThisYear = await _accountRepository.GetUsersCreatedBetweenDates(firstDayOfYear, today.AddDays(1));

            // Tính tổng số lượng mới cho mỗi khoảng thời gian
            var totalToday = usersCreatedToday.Count();
            var totalMonth = usersCreatedThisMonth.Count();
            var totalYear = usersCreatedThisYear.Count();

            // Tính phần trăm tăng so với ngày hôm qua
            // Lấy tổng số lượng người dùng được tạo ra ngày hôm qua
            var yesterday = today.AddDays(-1);
            var usersCreatedYesterday = await _accountRepository.GetUsersCreatedBetweenDates(yesterday, today);
            var totalYesterday = usersCreatedYesterday.Count();

            // Tính phần trăm tăng
            var percentIncreaseToday = Math.Abs(totalYesterday) < double.Epsilon ? totalToday * 100.0 : ((double)totalToday / totalYesterday - 1) * 100;


            // Tính phần trăm tăng so với tháng trước
            // Lấy tổng số lượng người dùng được tạo ra tháng trước
            var firstDayOfLastMonth = firstDayOfMonth.AddMonths(-1);
            var lastMonth = today.AddMonths(-1).Month;
            var lastMonthYear = today.AddMonths(-1).Year;
            var usersCreatedLastMonth = await _accountRepository.GetUsersCreatedBetweenDates(firstDayOfLastMonth, firstDayOfMonth);
            var totalLastMonth = usersCreatedLastMonth.Count();

            // Tính phần trăm tăng
            var percentIncreaseMonth = totalLastMonth == 0 ? totalMonth * 100.0 : ((double)totalMonth / totalLastMonth - 1) * 100;

            // Khởi tạo DTO và trả về
            var newCustomerDTO = new GetNewCustomerDTO
            {
                Today = totalToday,
                PercentIncreaseToday = percentIncreaseToday,
                ThisMonth = totalMonth,
                PercentIncreaseMonth = percentIncreaseMonth,
                ThisYear = totalYear
            };

            return newCustomerDTO;
        }

        public async Task<SignInResultDTO> SignInAdminAsync(SignIn signIn)
        {
            var isEmail = signIn.EmailOrUsername.Contains("@");
            var user = new AppUser();
            if (isEmail)
            {
                user = await _accountRepository.GetUserByEmailAsync(signIn.EmailOrUsername);
            }
            else
            {
                user = await _accountRepository.GetUserByUserNameAsync(signIn.EmailOrUsername);
            }
            if (user == null && isEmail)
            {
                throw new BadRequestException("Email không tồn tại");
            }
            if (user == null && !isEmail)
            {
                throw new BadRequestException("Tên đăng nhập không tồn tại");
            }
            var checkPassword = await _accountRepository.CheckPasswordAsync(user, signIn.Password);
            if (!checkPassword)
            {
                throw new BadRequestException("Mật khẩu không chính sát");
            }
            // if(!user.EmailConfirmed){
            //      var confirmEmailToken = await _accountRepository.GenerateEmailConfirmationTokenAsync(user);
            //      var confirmationLink = GenerateConfirmationLink(user, confirmEmailToken);
            //     await _emailService.SendEmailAsync(user.Email, "Xác nhận email của bạn", $"Vui lòng xác nhận email của bạn bằng cách nhấp vào liên kết này: <a href=\"{confirmationLink}\">liên kết này</a>");
            //     throw new BadRequestException("Bạn chưa xác thực Email.Vui lòng vào Gmail để xác thực");
            // }
            var roles = await _accountRepository.GetUserRolesAsync(user.Id);
            if (roles == null || !(roles.Contains(AppRole.Admin) || roles.Contains(AppRole.SuperAdmin)))
            {
                throw new ForbiddenAccessException("Bạn không có quyền truy cập vào hệ thống");
            }
            var token = await _accountRepository.GenerateJwtToken(user, signIn.RememberMe);
            var claims = await _accountRepository.GetUserClaimsAsync(user.Id);

            var signInResult = new SignInResultDTO
            {
                User = _mapper.Map<UserGetDTO>(user),
                Token = token
            };
            signInResult.User.Roles = roles;
            signInResult.User.Claims = claims;
            return signInResult;
        }
        public async Task<SignInResultDTO> SignInAsync(SignIn signIn)
        {
            var isEmail = signIn.EmailOrUsername.Contains("@");
            var user = new AppUser();
            if (isEmail)
            {
                user = await _accountRepository.GetUserByEmailAsync(signIn.EmailOrUsername);
            }
            else
            {
                user = await _accountRepository.GetUserByUserNameAsync(signIn.EmailOrUsername);
            }
            if (user == null && isEmail)
            {
                throw new BadRequestException("Email không tồn tại");
            }
            if (user == null && !isEmail)
            {
                throw new BadRequestException("Tên đăng nhập không tồn tại");
            }
            if(user.PasswordHash!=null){
                  var checkPassword = await _accountRepository.CheckPasswordAsync(user, signIn.Password);
            if (!checkPassword)
            {
                throw new BadRequestException("Mật khẩu không chính sát");
            }
            }else{
                throw new BadRequestException("Vui lòng chọn Google để đăng nhập");
            }
          
            await _accountRepository.PasswordSignInAsync(user.Email, signIn.Password);
            var roles = await _accountRepository.GetUserRolesAsync(user.Id);
            var token = await _accountRepository.GenerateJwtToken(user, signIn.RememberMe);
            var claims = await _accountRepository.GetUserClaimsAsync(user.Id);

            var signInResult = new SignInResultDTO
            {
                User = _mapper.Map<UserGetDTO>(user),
                Token = token
            };
            signInResult.User.Roles = roles;
            signInResult.User.Claims = claims;
            return signInResult;
        }

        public async Task<IdentityResult> SignUpAsync(SignUp signUp)
        {
            var existingUser = await _accountRepository.GetUserByEmailAsync(signUp.Email);
            if (existingUser != null)
            {
                throw new Exception("Email này đã được đăng ký");
            }
            var userCreated = await _accountRepository.SignUpUserAsync(signUp);
            if (userCreated == null)
            {
                throw new Exception("Mật Khẩu phải ít nhất 7 ký tự, 1 ký tự hoa, 1 số, 1 ký tự đặt biệt.");
            }
            var result = await _accountRepository.AddRoleToUserAsync(userCreated, AppRole.Customer);
            if (!result)
            {
                throw new Exception("Quyền này không tồn tại");
            }
            return IdentityResult.Success;
        }
        public async Task SendEmailConfirm(string id, string url)
        {
            var user = await _accountRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                throw new NotFoundException("Người dùng không tồn tại");
            }
            var confirmEmailToken = await _accountRepository.GenerateEmailConfirmationTokenAsync(user);
            if (confirmEmailToken == null)
            {
                throw new Exception("Có lỗi xảy ra");
            }
            var encodedToken = System.Net.WebUtility.UrlEncode(confirmEmailToken);
            var confirmationLink = $"{url}/{id}/{encodedToken}";
            await _emailService.SendEmailAsync(user.Email, "Xác nhận email của bạn", $"Vui lòng xác nhận email của bạn bằng cách nhấp vào liên kết này: <a href=\"{confirmationLink}\">liên kết này</a>");
        }
        public async Task SendRestPasswordConfirm(ResetPasswordInputDTO inputDTO)
        {
            var isEmail = inputDTO.EmailOrUsername.Contains("@");
            var user = new AppUser();
            if (isEmail)
            {
                user = await _accountRepository.GetUserByEmailAsync(inputDTO.EmailOrUsername);
            }
            else
            {
                user = await _accountRepository.GetUserByUserNameAsync(inputDTO.EmailOrUsername);
            }

            if (user == null)
            {
                throw new NotFoundException("Người dùng không tồn tại");
            }
            var confirmEmailToken = await _accountRepository.GeneratePasswordResetTokenAsync(user);
            if (confirmEmailToken == null)
            {
                throw new Exception("Có lỗi xảy ra");
            }
            var encodedToken = System.Net.WebUtility.UrlEncode(confirmEmailToken);
            var confirmationLink = $"{inputDTO.CurrentHost}/{user.Email}/{encodedToken}";
            await _emailService.SendEmailAsync(user.Email, "Xác nhận email của bạn", $"Nếu bạn muốn đặt lại mật khẩu của bạn hãy nhấp vào liên kết này: <a href=\"{confirmationLink}\">liên kết này</a>");
        }
        public async Task<bool> ConfirmEmailAsync(string userId, string confirmEmailToken)
        {

            // Kiểm tra thông tin xác thực
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(confirmEmailToken))
            {
                throw new BadRequestException("Đường đẫn có vấn đề.");
            }
            // Tìm người dùng
            var user = await _accountRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("Người dùng không tồn tại , vui lòng đăng ký lại.");
            }
            var confirmed = await _accountRepository.ConfirmEmailAsync(user, confirmEmailToken);
            if (!confirmed)
            {
                throw new BadRequestException("Đường dẫn hết hạn hoặc có lỗi xảy ra!");
            }
            return confirmed;
        }
        public async Task<bool> ConfirmSetPasswordAsync(SetPasswordInputDTO inputDTO)
        {

            // Kiểm tra thông tin xác thực
            if (string.IsNullOrEmpty(inputDTO.Email) || string.IsNullOrEmpty(inputDTO.Token))
            {
                throw new BadRequestException("Đường đẫn có vấn đề.");
            }
            // Tìm người dùng
            var user = await _accountRepository.GetUserByEmailAsync(inputDTO.Email);
            if (user == null)
            {
                throw new NotFoundException("Email sai hoặc không tông tại. Vui lòng tự nhận liên kết mới.");
            }
            var confirmed = await _accountRepository.ResetPasswordAsync(user, inputDTO.Token, inputDTO.Password);
            if (!confirmed)
            {
                throw new BadRequestException("Đường dẫn hết hạn hoặc có lỗi xảy ra!");
            }
            return confirmed;
        }
        private string GenerateConfirmationLink(AppUser user, string confirmEmailToken)
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            var urlActionContext = new UrlActionContext
            {
                Action = "ConfirmEmail",
                Controller = "Account",
                Values = new { userId = user.Id, confirmEmailToken = confirmEmailToken },
                Protocol = _actionContextAccessor.ActionContext.HttpContext.Request.Scheme
            };
            var confirmationLink = urlHelper.Action(urlActionContext);
            return confirmationLink;
        }

        public async Task<PagedResult<UserGetDTO>> GetUsersAsync(ProductAdminFilterDTO filterDTO, string email)
        {
            var users = await _accountRepository.GetUsersAsync(AppRole.Admin,filterDTO, email);
            var usersDTO = new List<UserGetDTO>();

            foreach (var user in users.Items)
            {
                var roles = await _accountRepository.GetUserRolesAsync(user.Id);
                if (!roles.Contains(AppRole.Admin) && !roles.Contains(AppRole.SuperAdmin))
                {
                    continue;
                }
                var claims = await _accountRepository.GetUserClaimsAsync(user.Id);

                usersDTO.Add(new UserGetDTO
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Avatar = user.Avatar,
                    EmailConfirmed = user.EmailConfirmed,
                    Roles = roles,
                    Claims = claims
                });
                // users.TotalCount--;

            }
                return new PagedResult<UserGetDTO>
    {
        Items = usersDTO,
        TotalCount = users.TotalCount,
        PageSize = users.PageSize,
        CurrentPage = users.CurrentPage
    };
        }
        public async Task<PagedResult<UserGetDTO>> GetCusomerAsync([FromQuery] ProductAdminFilterDTO filterDTO, string email)
        {
            var users = await _accountRepository.GetUsersAsync(AppRole.Customer,filterDTO, email);
            var usersDTO = new List<UserGetDTO>();

            foreach (var user in users.Items)
            {
                var roles = await _accountRepository.GetUserRolesAsync(user.Id);
                if (roles.Contains(AppRole.Admin) || roles.Contains(AppRole.SuperAdmin))
                {
                    continue;
                }
                var claims = await _accountRepository.GetUserClaimsAsync(user.Id);

                usersDTO.Add(new UserGetDTO
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Avatar = user.Avatar,
                    EmailConfirmed = user.EmailConfirmed,
                    Roles = roles,
                    Claims = claims
                });
                // users.TotalCount--;
            }
                            return new PagedResult<UserGetDTO>
    {
        Items = usersDTO,
        TotalCount = users.TotalCount,
        PageSize = users.PageSize,
        CurrentPage = users.CurrentPage
    };
        }
        public async Task CUExistingUser(string idCreate, string idUpdate)
        {
            var checkCreatingUser = await _accountRepository.GetUserByIdAsync(idCreate);
            if (checkCreatingUser == null)
            {
                throw new UnauthorizedAccessException("Bạn cần đăng nhập lại!");
            }

            var checkUpdatingUser = await _accountRepository.GetUserByIdAsync(idUpdate);
            if (checkUpdatingUser == null)
            {
                throw new UnauthorizedAccessException("Bạn cần đăng nhập lại!");
            }
        }
        public async Task<UserGetDTO> GetUserByIdAsync(string id)
        {
            var user = await _accountRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                throw new NotFoundException("Người dùng không tồn tại");
            }
            var userDTO = _mapper.Map<UserGetDTO>(user);
            userDTO.Roles = await _accountRepository.GetUserRolesAsync(userDTO.Id);
            userDTO.Claims = await _accountRepository.GetUserClaimsAsync(userDTO.Id);
            return userDTO;
        }
        public async Task<UserGetDTO> GetMyUserAsync(string token)
        {
            var userId = ExtractUserIdFromToken(token);
            if (userId == null)
            {
                throw new NotFoundException("Có lỗi xãy ra vui lòng đăng nhập lại");
            }
            var user = await _accountRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("Người dùng không tồn tại");
            }

            var userDTO = _mapper.Map<UserGetDTO>(user);
            userDTO.Roles = await _accountRepository.GetUserRolesAsync(userDTO.Id);
            userDTO.Claims = await _accountRepository.GetUserClaimsAsync(userDTO.Id);
            return userDTO;
        }
        public async Task<bool> CheckUserIdMatchesEmail(string id, string email)
        {
            var user = await _accountRepository.GetUserByEmailAsync(email);
            if (user.Id == id)
            {
                return true;
            }
            return false;
        }
        public async Task<bool> ChangeStatusUserAsync(string id)
        {
            var now = DateTime.UtcNow;
            now = TimeZoneInfo.ConvertTimeFromUtc(now, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            var user = await _accountRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                throw new NotFoundException("Người dùng không tồn tại");
            }
            IEnumerable<string> roles;
            roles = await _accountRepository.GetUserRolesAsync(user.Id);
            if (roles.Contains(AppRole.SuperAdmin))
            {
                throw new BadRequestException("Bạn không có quyền làm đều này");
            }
            user.EmailConfirmed = user.EmailConfirmed == true ? false : true;
            user.UpdatedAt = now;
            var updated = await _accountRepository.UpdateUserAsync(id, user);
            if (!updated)
            {
                throw new BadRequestException("Có lỗi xảy ra");
            }
            return true;
        }
        public async Task<bool> UpdateUserByAdminAsync(string userId, UserUpdateByAdminDTO userUpdateDto, IFormFile avatar)
        {
            var now = DateTime.UtcNow;
            now = TimeZoneInfo.ConvertTimeFromUtc(now, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            var user = await _accountRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("Người dùng không tồn tại");
            }
            IEnumerable<string> roles;
            roles = await _accountRepository.GetUserRolesAsync(user.Id);
            if (roles.Contains(AppRole.SuperAdmin))
            {
                throw new BadRequestException("Bạn không có quyền làm đều này");
            }
            if (user.UserName != userUpdateDto.UserName)
            {
                var existingUsername = await _accountRepository.GetUserByUserNameAsync(userUpdateDto.UserName);
                if (existingUsername != null)
                {
                    throw new NotFoundException("Tên này đã được dùng");
                }
            }
            if (user.Email != userUpdateDto.Email)
            {
                var existingUserEmail = await _accountRepository.GetUserByEmailAsync(userUpdateDto.Email);
                if (existingUserEmail != null)
                {
                    throw new Exception("Email này đã được đăng ký");
                }
                else
                {
                    user.EmailConfirmed = false;
                }
            }
            if (userUpdateDto.Password != null)
            {
                var resetToken = await _accountRepository.GeneratePasswordResetTokenAsync(user);
                var result = await _accountRepository.ResetPasswordAsync(user, resetToken, userUpdateDto.Password);
                if (!result)
                {
                    throw new Exception("Không thể cập nhật mật khẩu");
                }
            }
            user.FirstName = userUpdateDto.FirstName;
            user.LastName = userUpdateDto.LastName;
            user.UserName = userUpdateDto.UserName;
            user.Email = userUpdateDto.Email;
            user.PhoneNumber = userUpdateDto.PhoneNumber;
            user.Gender = userUpdateDto.Gender;
            if (avatar != null)
            {
                if (user.Avatar != "avatar-nam.jpg" && user.Avatar != "avatar-nu.jpg")
                {
                    await _galleryService.DeleteImageAsync(user.Avatar, "users");
                }
                var AvatarUpload = await _galleryService.UploadImage(userUpdateDto.LastName, avatar, "users");
                user.Avatar = AvatarUpload;
            }
            else if (user.Avatar == "avatar-nam.jpg" || user.Avatar == "avatar-nu.jpg")
            {
                user.Avatar = userUpdateDto.Gender == true ? "avatar-nam.jpg" : "avatar-nu.jpg";

            }
            user.UpdatedAt = now;
            var updated = await _accountRepository.UpdateUserAsync(userId, user);
            if (!updated)
            {
                throw new BadRequestException("Có lỗi xảy ra");
            }
            if (userUpdateDto.Roles.Count > 0)
            {
                var result = await _accountRepository.UpdateUserRolesAsync(userId, userUpdateDto.Roles);
                if (!result)
                {
                    throw new Exception("Quyền này không tồn tại");
                }
            }
            else
            {
                var result = await _accountRepository.UpdateUserRolesAsync(userId, [AppRole.Customer]);
                if (!result)
                {
                    throw new Exception("Quyền này không tồn tại");
                }
            }
            var deleteClaims = await _accountRepository.DeleteClaimsAsync(userId);
            if (deleteClaims)
            {
                foreach (var claim in userUpdateDto.Claims)
                {
                    if(claim.ClaimValues !=null){
                         await _accountRepository.AddClaimToUserAsync(userId, claim.ClaimType, claim.ClaimValues);
                    }
                   
                }
            }

            return true;
        }
        public async Task<UserGetDTO> UpdateMyUserAsync(string userId, MyUserUpdateDTO userUpdateDto, IFormFile avatar)
        {
            var now = DateTime.UtcNow;
            now = TimeZoneInfo.ConvertTimeFromUtc(now, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            var user = await _accountRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("Người dùng không tồn tại");
            }
            if (user.UserName != userUpdateDto.UserName && userUpdateDto.UserName != null)
            {
                var existingUsername = await _accountRepository.GetUserByUserNameAsync(userUpdateDto.UserName);
                if (existingUsername != null)
                {
                    throw new NotFoundException("Tên này đã được dùng");
                }
            }
            if (user.Email != userUpdateDto.Email)
            {
                var existingUserEmail = await _accountRepository.GetUserByEmailAsync(userUpdateDto.Email);
                if (existingUserEmail != null)
                {
                    throw new Exception("Email này đã được đăng ký");
                }
                else
                {
                    user.EmailConfirmed = false;
                }
            }
            if (userUpdateDto.OldPassword != null)
            {
                var checkPassword = await _accountRepository.CheckPasswordAsync(user, userUpdateDto.OldPassword);
                if (!checkPassword)
                {
                    throw new NotFoundException("Mật khẩu cũ không khớp");
                }
                if (userUpdateDto.Password != null)
                {
                    var resetToken = await _accountRepository.GeneratePasswordResetTokenAsync(user);
                    var result = await _accountRepository.ResetPasswordAsync(user, resetToken, userUpdateDto.Password);
                    if (!result)
                    {
                        throw new Exception("Không thể cập nhật mật khẩu");
                    }
                }
            }

            user.FirstName = userUpdateDto.FirstName;
            user.LastName = userUpdateDto.LastName;
            if (userUpdateDto.UserName != null)
            {
                user.UserName = userUpdateDto.UserName;
            }
            user.Email = userUpdateDto.Email;
            user.PhoneNumber = userUpdateDto.PhoneNumber;
            user.Gender = userUpdateDto.Gender;
            if (avatar != null)
            {
                if (user.Avatar != "avatar-nam.jpg" && user.Avatar != "avatar-nu.jpg")
                {
                    await _galleryService.DeleteImageAsync(user.Avatar, "users");
                }
                var AvatarUpload = await _galleryService.UploadImage(userUpdateDto.LastName, avatar, "users");
                user.Avatar = AvatarUpload;
            }
            else if (user.Avatar == "avatar-nam.jpg" || user.Avatar == "avatar-nu.jpg")
            {
                user.Avatar = userUpdateDto.Gender == true ? "avatar-nam.jpg" : "avatar-nu.jpg";

            }
            user.UpdatedAt = now;
            var updated = await _accountRepository.UpdateUserAsync(userId, user);
            if (!updated)
            {
                throw new BadRequestException("Có lỗi xảy ra");
            }
            var userDTO = _mapper.Map<UserGetDTO>(user);
            userDTO.Roles = await _accountRepository.GetUserRolesAsync(userDTO.Id);
            userDTO.Claims = await _accountRepository.GetUserClaimsAsync(userDTO.Id);
            return userDTO;
        }

        public string ExtractEmailFromToken(string tokenWithBearer)
        {
            var token = tokenWithBearer.Replace("Bearer ", "");
            // Khởi tạo đối tượng JwtSecurityTokenHandler để xử lý token
            var tokenHandler = new JwtSecurityTokenHandler();

            // Giải mã token
            var decodedToken = tokenHandler.ReadJwtToken(token);

            // Trích xuất claim chứa ID của người dùng
            var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "email");
            return userIdClaim?.Value;
        }
        public string ExtractUserIdFromToken(string tokenWithBearer)
        {
            var token = tokenWithBearer.Replace("Bearer ", "");
            // Khởi tạo đối tượng JwtSecurityTokenHandler để xử lý token
            var tokenHandler = new JwtSecurityTokenHandler();

            // Giải mã token
            var decodedToken = tokenHandler.ReadJwtToken(token);

            // Trích xuất claim chứa ID của người dùng
            var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "nameid");
            return userIdClaim?.Value;
        }
        public async Task DeleteUserById(string id)
        {

            var user = await _accountRepository.GetUserByIdAsync(id);

            if (user != null)
            {
                IEnumerable<string> roles;
                roles = await _accountRepository.GetUserRolesAsync(user.Id);
                if (roles.Contains(AppRole.SuperAdmin))
                {
                    throw new BadRequestException("Bạn không có quyền làm đều này");
                }
                if (user.Avatar != "avatar-nam.jpg" && user.Avatar != "avatar-nu.jpg")
                {
                    await _galleryService.DeleteImageAsync(user.Avatar, "users");
                }
            }
            var deleted = await _accountRepository.DeleteUserByIdAsync(id);

            if (!deleted)
            {
                throw new NotFoundException("Người dùng không tồn tại");
            }
        }
        public async Task DeleteUsersById(List<string> ids)
        {

            foreach (var id in ids)
            {
                var existing = await _accountRepository.GetUserByIdAsync(id);
                if (existing != null)
                {
                    if (existing.Avatar != "avatar-nam.jpg" && existing.Avatar != "avatar-nu.jpg")
                    {
                        await _galleryService.DeleteImageAsync(existing.Avatar, "users");
                    }

                    await _accountRepository.DeleteUserByIdAsync(id);
                }
            }

        }

        public async Task<UserGetDTO> CreateUserAsync(UserCreateDTO userCreateDto)
        {
            var existingUserEmail = await _accountRepository.GetUserByEmailAsync(userCreateDto.Email);
            if (existingUserEmail != null)
            {
                throw new Exception("Email này đã được đăng ký");
            }
            if (userCreateDto.Roles.Contains(AppRole.SuperAdmin))
            {
                throw new BadRequestException("Bạn không có quyền làm đều này");
            }
            var userCreated = await _accountRepository.CreateUserAsync(userCreateDto);
            if (userCreated == null)
            {
                throw new Exception("Mật Khẩu phải ít nhất 7 ký tự, 1 ký tự hoa, 1 số, 1 ký tự đặt biệt.");
            }
            if (userCreateDto.Roles.Count > 0)
            {
                var result = await _accountRepository.AddRolesToUserAsync(userCreated, userCreateDto.Roles);
                if (!result)
                {
                    throw new Exception("Quyền này không tồn tại");
                }
            }
            else
            {
                var result = await _accountRepository.AddRoleToUserAsync(userCreated, AppRole.Customer);
                if (!result)
                {
                    throw new Exception("Quyền này không tồn tại");
                }
            }
            foreach (var claim in userCreateDto.Claims)
            {
                                    if(claim.ClaimValues !=null){
await _accountRepository.AddClaimToUserAsync(userCreated.Id, claim.ClaimType, claim.ClaimValues);
                                    }

                
            }


            return _mapper.Map<UserGetDTO>(userCreated);
        }
        public async Task<List<string?>> GetRolesWithTempIdsAsync()
        {
            var roles = await _accountRepository.GetAllRolesAsync();

            return roles;
        }


    }

}