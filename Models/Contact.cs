
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace backend.Models
{
    public class Contact:DateTimeInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id {get;set;}
        public  string? Name { get; set; }
        public string? Email {get;set;}
        public string? Phone {get;set;}
        public string? Content {get;set;}
        public string? ReplayContent {get;set;}
        public int Status {get;set;}=0;
        public string? UpdatedById {get;set;}
        public virtual AppUser? UpdatedBy {get;set;}

    }
}