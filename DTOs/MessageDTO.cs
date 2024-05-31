using System.ComponentModel.DataAnnotations;
namespace backend.DTOs
{
    public class MessageDTO
    {
        public long Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UserId { get; set; }
        public UserGetShortDTO User {get;set;}
    }
}