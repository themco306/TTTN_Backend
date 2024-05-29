using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models
{
    public class Product : DateTimeInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long CategoryId {get;set;}
        public virtual Category Category { get; set; }

        public long? BrandId {get;set;}
        public virtual Brand? Brand { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Slug { get; set; }

        public string Description { get; set; }

        public string Detail { get; set; }

        public decimal SalePrice { get; set; }

        public double Star { get;set;}

        public decimal ComparePrice { get; set; }

        public decimal BuyingPrice { get; set; }

        public int Quantity { get; set; }

        public long TotalItemsSold {get;set;}=0;

        [Column(TypeName = "varchar(64)")]
        public string ProductType { get; set; }

        public string? Note { get; set; }
        public int Status { get; set; }
        public string? CreatedById { get; set; }
        public virtual AppUser? CreatedBy { get; set; }

        public string? UpdatedById { get; set; }
        public virtual AppUser? UpdatedBy { get; set; }

        // [JsonIgnore]
        public virtual List<Gallery> Galleries { get; set; } = new List<Gallery>();
        [JsonIgnore]
        public List<ProductTag> ProductTags { get; set; }


        // Navigation property for the Category
        
    }
}
