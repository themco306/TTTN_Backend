using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace backend.Models
{
    public enum DiscountType
    {
        Percentage,
        FixedAmount
    }
    public class ApplicableProducts
    {
        public List<long>? CategoryIds { get; set; }
        public List<long>? ProductIds { get; set; }
    }
    public class Coupon : DateTimeInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public string Code { get; set; }

        public string? Description { get; set; }

       [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DiscountType DiscountType { get; set; }

        [Required]
        public decimal DiscountValue { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public int UsageLimit { get; set; } = 0;

        public int UsagePerUser { get; set; } = 0;

        public decimal MinimumOrderValue { get; set; } = 0.0m;

        [NotMapped]
        public ApplicableProducts? ApplicableProducts { get; set; }

        [JsonIgnore]
        public string? ApplicableProductsJson
        {
            get => ApplicableProducts != null ? JsonSerializer.Serialize(ApplicableProducts) : null;
            set => ApplicableProducts = value != null ? JsonSerializer.Deserialize<ApplicableProducts>(value) : null;
        }

        public int Status { get; set; } =0; 

        public string? CreatedById { get; set; }
        public virtual AppUser? CreatedBy { get; set; }

        public string? UpdatedById { get; set; }
        public virtual AppUser? UpdatedBy { get; set; }

        public List<CouponUsage> CouponUsages {get;set;} 
    }
}
