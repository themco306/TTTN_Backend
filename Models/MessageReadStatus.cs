
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace backend.Models
{
    public class MessageReadStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long MessageId { get; set; }
        [JsonIgnore]
        public Message Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; }
        [JsonIgnore]
        public AppUser User { get; set; }

    }
}
