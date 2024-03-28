using System;
using System.Collections.Generic;
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
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Org.BouncyCastle.Asn1.Ocsp;

namespace backend.Services
{
    public class AccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly EmailService _emailService;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IMapper _mapper;

        public AccountService(IAccountRepository accountRepository, EmailService emailService, IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor,IMapper mapper)
        {
            _accountRepository = accountRepository;
            _emailService = emailService;
            _urlHelperFactory = urlHelperFactory;
            _actionContextAccessor = actionContextAccessor;
            _mapper=mapper;
        }
        public async Task<string> SignInAsync(SignIn signIn)
        {
            var user = await _accountRepository.GetUserByEmailAsync(signIn.Email);
            if (user == null)
            {
                throw new Exception("Tên đăng nhập không tồn tại");
            }
            var checkPassword = await _accountRepository.CheckPasswordAsync(user, signIn.Password);
            if (!checkPassword)
            {
                throw new Exception("Mật khẩu không chính sát");
            }
            var result = await _accountRepository.PasswordSignInAsync(signIn.Email, signIn.Password);
            if (!result.Succeeded)
            {
                throw new Exception("Đăng nhập thất bại");
            }
            return await _accountRepository.GenerateJwtToken(user);
        }

        public async Task<IdentityResult> SignUpAsync(SignUp signUp)
        {
            var existingUser =await _accountRepository.GetUserByEmailAsync(signUp.Email);
            if(existingUser!=null){
                throw new Exception("Email này đã được đăng ký");
            }
            var userCreated = await _accountRepository.CreateUserAsync(signUp);
            if (userCreated == null)
            {
                throw new Exception("Mật Khẩu phải ít nhất 7 ký tự, 1 ký tự hoa, 1 số, 1 ký tự đặt biệt.");
            }
            var result = await _accountRepository.AddRoleToUserAsync(userCreated, AppRole.Customer);
            if (!result)
            {
                throw new Exception("Quyền này không tồn tại");
            }
            var confirmEmailToken = await _accountRepository.GenerateEmailConfirmationTokenAsync(userCreated);
            if (confirmEmailToken == null)
            {
                throw new Exception("Có lỗi xảy ra");
            }
            else
            {
                var confirmationLink = GenerateConfirmationLink(userCreated, confirmEmailToken);
                await _emailService.SendEmailAsync(userCreated.Email, "Xác nhận email của bạn", $"Vui lòng xác nhận email của bạn bằng cách nhấp vào liên kết này: <a href=\"{confirmationLink}\">liên kết này</a>");

            }
            return IdentityResult.Success;
        }

         public async Task<bool> ConfirmEmailAsync(string userId, string confirmEmailToken){

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
            var confirmed = await _accountRepository.ConfirmEmailAsync(user,confirmEmailToken);
            if(!confirmed){
                throw new BadRequestException("Thất bại vui lòng thử lại");
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

        public async Task<IEnumerable<UserGetDTO>> GetUsersAsync(int pageIndex,int pageSize){
            var users =await _accountRepository.GetUsersAsync(pageIndex,pageSize);
             var usersDTO = new List<UserGetDTO>();

            foreach (var user in users)
            {
                var roles =  await _accountRepository.GetUserRolesAsync(user.Id);

                usersDTO.Add(new UserGetDTO
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    EmailConfirmed = user.EmailConfirmed,
                    Roles = roles 
                });
            }
            return usersDTO;
        }

        public async Task<UserGetDTO> GetUserByIdAsync(string id)
        {
            var user =await   _accountRepository.GetUserByIdAsync(id);
            if(user==null){
                throw new NotFoundException("Người dùng không tồn tại");
            }
            var userDTO =_mapper.Map<UserGetDTO>(user);
                userDTO.Roles = await _accountRepository.GetUserRolesAsync(userDTO.Id);
            return userDTO;
        }

        public async Task<bool> UpdateUserAsync(string userId, UserCustomerUpdateDTO userUpdateDto)
    {
            var user =await   _accountRepository.GetUserByIdAsync(userId);
            if(user==null){
                throw new NotFoundException("Người dùng không tồn tại");
            }

            // Cập nhật thông tin người dùng với dữ liệu từ DTO
            user.FirstName = userUpdateDto.FirstName;
            user.LastName = userUpdateDto.LastName;
            user.Email = userUpdateDto.Email;
            user.PhoneNumber = userUpdateDto.PhoneNumber;

        var isConfirmed= await _accountRepository.CheckEmailConfirmedAsync(user);
        if(!isConfirmed){
            throw new BadRequestException("Bạn cần xác thực Email để thực hiện.");
        }

        var updated=await _accountRepository.UpdateUserAsync(userId, user);
        if(!updated){
            throw new BadRequestException("Có lỗi xảy ra");
        }
        return true;
    }
        public async  Task DeleteUserById(string id)
        {
            var deleted= await _accountRepository.DeleteUserByIdAsync(id);
            if(!deleted){
                throw new NotFoundException("Người dùng không tồn tại");
            }
        }
    }

}