using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class Product : DateTimeInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long CategoryId {get;set;}
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Slug { get; set; }

        [StringLength(165)]
        public string Description { get; set; }

        public string Detail { get; set; }

        public decimal SalePrice { get; set; }

        public decimal ComparePrice { get; set; }

        public decimal BuyingPrice { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "varchar(64)")]
        public string ProductType { get; set; }

        public string? Note { get; set; }
        public int Status { get; set; }
        public string? CreatedById { get; set; }
        [ForeignKey("CreatedById")]
        public virtual AppUser? CreatedBy { get; set; }

        public string? UpdatedById { get; set; }
        [ForeignKey("UpdatedById")]
        public virtual AppUser? UpdatedBy { get; set; }

        // Navigation property for the Category
        
    }
}
