
using System.ComponentModel.DataAnnotations;
namespace backend.DTOs
{
    public class SignIn
    {
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ Email hoặc tên đăng nhập.")]
        public string EmailOrUsername { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        public string Password { get; set; } = null!;
    }
}
