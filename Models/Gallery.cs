using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class Gallery : DateTimeInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long ProductId { get; set; }
        [Required]
        public string Image { get; set; }
        [Required]
        public string Placeholder { get; set; }
        public int Order { get; set; } 

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}
