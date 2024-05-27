

using System.ComponentModel.DataAnnotations;
using backend.Models;

namespace backend.DTOs
{
    public class ContactInputDTO
    {

        [Required(ErrorMessage = "Vui lòng nhập tên.")]
        [StringLength(50, ErrorMessage = "Tên không được vượt quá 50 ký tự.")]
        public string Name { get; set; }
        public string Email {get;set;}
        public string Phone {get;set;}
        public string Content {get;set;}

    }
        public class ContactReplayDTO
    {
        public string ReplayContent {get;set;}

    }

    public class ContactGetDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email {get;set;}
        public string Phone {get;set;}
        public string Content {get;set;}
        public string? ReplayContent {get;set;}
        public int Status { get; set; }
        public  UserGetShortDTO UpdatedBy {get;set;}
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


    }

}