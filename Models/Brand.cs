
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace backend.Models
{
    // [Table("Category")]
    public class Brand:DateTimeInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id {get;set;}

        [Required]
        public  string? Name { get; set; }

        [Required]
        public string? Slug {get;set;}

        public int Status {get;set;}=0;

        public string? CreatedById {get;set;}
        [JsonIgnore]

        public virtual AppUser? CreatedBy {get;set;}

        public string? UpdatedById {get;set;}
        [JsonIgnore]

        public virtual AppUser? UpdatedBy {get;set;}
       [JsonIgnore]
        public virtual List<Product> Products {get;set;}=new List<Product>();

    }
}