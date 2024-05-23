

using System.ComponentModel.DataAnnotations;
using backend.Models;

namespace backend.DTOs
{
    public class BrandInputDTO
    {

        [Required(ErrorMessage = "Vui lòng nhập tên.")]
        [StringLength(50, ErrorMessage = "Tên không được vượt quá 50 ký tự.")]
        public string Name { get; set; }
        public int Status { get; set; }

    }

    public class BrandGetDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int Status { get; set; }
        public  UserGetShortDTO CreatedBy {get;set;}
        public  UserGetShortDTO UpdatedBy {get;set;}
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


    }

}