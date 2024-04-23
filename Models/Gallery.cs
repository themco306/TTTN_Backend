using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models
{
    public class Gallery : DateTimeInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long ProductId { get; set; }
        [Required]
        public string ImageName { get; set; } 

        public string ImagePath { get; set; }
        [Required]
        public string Placeholder { get; set; }
        public int Order { get; set; } 

        [ForeignKey("ProductId")]
        [JsonIgnore]
        public  Product Product { get; set; }
    }
}
