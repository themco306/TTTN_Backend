

using System.ComponentModel.DataAnnotations;
using backend.Models;

namespace backend.DTOs
{
    public class CategoryInputDTO
    {
        public long? ParentId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên.")]
        [StringLength(50, ErrorMessage = "Tên không được vượt quá 50 ký tự.")]
        public string Name { get; set; }

        [StringLength(255, ErrorMessage = "Mô tả không được vượt quá 255 ký tự.")]
        public string? Description { get; set; }

        public int? Status { get; set; }

        [Required(ErrorMessage = "Thiếu CreatedById")]
        public string CreatedById { get; set; }
        [Required(ErrorMessage = "Thiếu UpdatedById")]
        public string UpdatedById { get; set; }

    }

    public class CategoryGetDTO
    {
        public long Id { get; set; }
        public long? ParentId { get; set; } = null;
        public string Name { get; set; }

        public string Slug { get; set; }

        public string Description { get; set; }
        public int Status { get; set; }
        public  AppUser CreatedBy {get;set;}
        public  AppUser UpdatedBy {get;set;}

        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }

        public int TotalProduct {get;set;}

    }

}