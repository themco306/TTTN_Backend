
using System.ComponentModel.DataAnnotations;
namespace backend.DTOs
{
    public class UserCreateDTO
    {
        [Required(ErrorMessage = "Vui lòng nhập tên.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ email.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Mật khẩu không khớp.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Số điện thoại chỉ được chứa các ký tự số.")]
        [StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự.")]
        public string PhoneNumber { get; set; }

        public bool? Gender { get; set; }

        public List<string> Roles { get; set; }

        public List<ClaimDTO> Claims { get; set; }

    }
    public class UserGetDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Avatar { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool Gender { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public List<ClaimDTO> Claims { get; set; }
    }
    public class UserGetShortDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Id { get; set; }
       
    }



    public class UserUpdateByAdminDTO
    {
        [Required(ErrorMessage = "Vui lòng nhập tên.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập.")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Tên đăng nhập chỉ được chứa chữ thường, chữ hoa và số.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ email.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Compare("Password", ErrorMessage = "Mật khẩu không khớp.")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }

        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Số điện thoại chỉ được chứa các ký tự số.")]
        [StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự.")]
        public string PhoneNumber { get; set; }


        public bool? Gender { get; set; }

        public List<string>? Roles { get; set; }

        public List<ClaimDTO>? Claims { get; set; }

    }
    public class MyUserUpdateDTO
    {
        [Required(ErrorMessage = "Vui lòng nhập tên.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ.")]
        public string LastName { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Tên đăng nhập chỉ được chứa chữ thường, chữ hoa và số.")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ email.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string? OldPassword { get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Compare("Password", ErrorMessage = "Mật khẩu không khớp.")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }

        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Số điện thoại chỉ được chứa các ký tự số.")]
        [StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự.")]
        public string PhoneNumber { get; set; }


        public bool? Gender { get; set; }


    }
    public class ResetPasswordInputDTO {
        public string EmailOrUsername {get;set;}
        public string CurrentHost {get;set;}
    }
       public class SetPasswordInputDTO {
        public string Email {get;set;}
        public string Token {get;set;}
        [DataType(DataType.Password)]
        public string Password {get;set;}
        
         [Compare("Password", ErrorMessage = "Mật khẩu không khớp.")]
        [DataType(DataType.Password)]

        public string ConfirmPassword {get;set;}
    }

}