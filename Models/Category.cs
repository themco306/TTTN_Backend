
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

        public string? Image {get;set;}

        public string? Icon {get;set;}

        public long CreatedBy {get;set;}

        public long UpdatedBy {get;set;}

        [ForeignKey("ParentId")]
        public virtual Category? Parent {get;set;}

        public virtual ICollection<ProductCategory> ProductCategories {get;set;}=new List<ProductCategory>();

    }
}