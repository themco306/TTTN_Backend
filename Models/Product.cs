
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace backend.Models
{
    // [Table("Category")]
    public class Product:DateTimeInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id {get;set;}

        [Required]
        public  string? Name { get; set; }

        [Required]
        public string? Slug {get;set;}

        public string? Description {get;set;}

        public long CreatedBy {get;set;}

        public long UpdatedBy {get;set;}

        public virtual ICollection<ProductCategory> ProductCategories {get;set;}=new List<ProductCategory>();


    }
}