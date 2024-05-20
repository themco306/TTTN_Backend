using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using backend.Models;

namespace backend.DTOs
{
    public class SubmitCodeDTO
    {
        public string Code {get;set;}
        public LongIDsModel ProductIds {get;set;}

    }
    public class CouponInputDTO
    {
        [Required(ErrorMessage = "Mã giảm giá là bắt buộc.")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Loại giảm giá là bắt buộc.")]
          [JsonConverter(typeof(JsonStringEnumConverter))]
        public DiscountType DiscountType { get; set; }

        [Required(ErrorMessage = "Giá trị giảm giá là bắt buộc.")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá trị giảm giá phải lớn hơn hoặc bằng 0.")]
        public decimal DiscountValue { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc là bắt buộc.")]
        public DateTime EndDate { get; set; }

        public int UsageLimit { get; set; } = 0;

        public int UsagePerUser { get; set; } = 0;

        public decimal MinimumOrderValue { get; set; } = 0.0m;

        // Không cần thiết lập Required cho các trường không liên quan đến cơ sở dữ liệu

        public ApplicableProducts ApplicableProducts { get; set; }

        public int Status { get; set; } = 0;
    }
    public class CouponUsagesDTO {
        public string  UserName { get; set; }
        public DateTime UsedAt { get; set; }
        public string  OrderCode { get; set; } 
    }
    public class CouponGetDTO {
        
        public long Id {get;set;}
        public string Code { get; set; }

        public string Description { get; set; }

       [JsonConverter(typeof(JsonStringEnumConverter))]
        public DiscountType DiscountType { get; set; }

      
        public decimal DiscountValue { get; set; }

       
        public DateTime StartDate { get; set; }

      
        public DateTime EndDate { get; set; }

        public int UsageLimit { get; set; } = 0;

        public int UsagePerUser { get; set; } = 0;

        public decimal MinimumOrderValue { get; set; } = 0.0m;

        // Không cần thiết lập Required cho các trường không liên quan đến cơ sở dữ liệu

        public ApplicableProducts ApplicableProducts { get; set; }

        public int Status { get; set; } = 0;

        public List<CouponUsagesDTO> CouponUsages {get;set;}


    }

}
