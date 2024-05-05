using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models
{
    public class Slider : DateTimeInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }

        public int Status { get; set; }

        public string? CreatedById {get;set;}
        public virtual AppUser? CreatedBy {get;set;}

        public string? UpdatedById {get;set;}
        public virtual AppUser? UpdatedBy {get;set;}
        
    }
}
