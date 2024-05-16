using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class CouponUsage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public string UserId { get; set; } // Assuming UserId is a string, adjust type if necessary
        public virtual AppUser User { get; set; }

        [Required]
        public long CouponId { get; set; }
        public virtual Coupon Coupon { get; set; }

        [Required]
        public DateTime UsedAt { get; set; }

        public long? OrderId { get; set; } 
        public virtual Order? Order { get; set; } 
    }
}
