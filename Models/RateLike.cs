
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace backend.Models
{
    public class RateLike
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id {get;set;}
        public  bool IsLike { get; set; }
        public string? UserId {get;set;}
        [JsonIgnore]
        public virtual AppUser? User {get;set;}
        public long? RateId {get;set;}
        [JsonIgnore]
        public virtual Rate? Rate {get;set;}

    }
}