

namespace backend.DTOs
{
    public class WebInfoInputDTO
    { 
        public IFormFile? Icon { get; set; }

        // Tên cửa hàng
        public string? ShopName { get; set; }

        public string? Description {get;set;}

        // Số điện thoại
        public string? PhoneNumber { get; set; }

        // Email
        public string? Email { get; set; }

        // Địa chỉ
        public string? Address { get; set; }

        public string? GoogleMap {get;set;}

        // Giờ làm việc
        public string? WorkingHours { get; set; }

        // Liên kết Facebook
        public string? FacebookLink { get; set; }

        // Liên kết Instagram
        public string? InstagramLink { get; set; }

        // Liên kết Twitter
        public string? TwitterLink { get; set; }

    }
}