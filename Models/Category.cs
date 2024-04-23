
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace backend.Models
{
    // [Table("Category")]
    public class Category:DateTimeInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id {get;set;}

        public long? ParentId {get;set;}

        [Required]
        public  string? Name { get; set; }

        [Required]
        public string? Slug {get;set;}

        public string? Description {get;set;}

        public int Status {get;set;}=0;

        public string? CreatedById {get;set;}
        public virtual AppUser? CreatedBy {get;set;}

        public string? UpdatedById {get;set;}
        public virtual AppUser? UpdatedBy {get;set;}
        
        public virtual Category? Parent {get;set;}
    //    [JsonIgnore]
        // public virtual List<Product> Products {get;set;}=new List<Product>();

    }
}