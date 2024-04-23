
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

        public bool? Gender {get;set;}

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
        public bool Gender {get;set;}
         public IEnumerable<string> Roles { get; set; } 
         public List<ClaimDTO> Claims {get; set; }
    }



public class UserCustomerUpdateDTO
{
    [Required(ErrorMessage = "Vui lòng nhập tên.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập họ.")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ email.")]
    [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
    public string Email { get; set; }

    [RegularExpression(@"^[0-9]*$", ErrorMessage = "Số điện thoại chỉ được chứa các ký tự số.")]
    [StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự.")]
    public string PhoneNumber { get; set; }
}

}