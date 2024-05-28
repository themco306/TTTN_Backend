
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace backend.Models
{
    public class Rate:DateTimeInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id {get;set;}
        public  int? Star { get; set; }=1;
        public string? Content {get;set;}
        public int? Like {get;set;}=0;
        public int? Dislike {get;set;}=0;
        public int Status {get;set;}=0;
        public string? UserId {get;set;}
        [JsonIgnore]

        public virtual AppUser? User {get;set;}
        public long? ProductId {get;set;}
        [JsonIgnore]
        public virtual Product? Product {get;set;}

    }
}