using System;
using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public class GoogleLoginRequest
{
    public string ClientId { get; set; }

    public string Credential { get; set; }
}
    public class SignUp
    {
        [Required(ErrorMessage = "Vui lòng nhập tên.")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập họ.")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ email.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    [MinLength(7, ErrorMessage = "Mật khẩu phải có ít nhất 7 ký tự.")]
    [RegularExpression(@"^(?=.*\d)(?=.*[A-Z])(?=.*\W).*$", ErrorMessage = "Mật khẩu phải chứa ít nhất 1 ký tự số, 1 ký tự viết hoa và 1 ký tự đặc biệt.")]

        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu.")]
        [Compare(nameof(Password), ErrorMessage = "Mật khẩu xác nhận không khớp với mật khẩu đã nhập.")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
