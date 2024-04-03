
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


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

        public string? CreatedById {get;set;}
        [ForeignKey("CreatedById")]
        public virtual AppUser? CreatedBy {get;set;}

        public string? UpdatedById {get;set;}
        [ForeignKey("UpdatedById")]
        public virtual AppUser? UpdatedBy {get;set;}
        
        [ForeignKey("ParentId")]
        public virtual Category? Parent {get;set;}

        public virtual ICollection<Product> Products {get;set;}=new List<Product>();

    }
}