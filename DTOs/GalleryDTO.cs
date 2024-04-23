using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public class GalleryInputDTO
    {
        [Required(ErrorMessage = "Vui lòng nhập ID sản phẩm.")]
        public long? ProductId { get; set; }
        public List<IFormFile> Images {get;set;}
    }

    public class GalleryDTO
    {
        public long Id { get; set; }

        public long? ProductId { get; set; }

        public string ImagePath { get; set; }
        public string ImageName { get; set; }

        public string Placeholder { get; set; }

        public int Order { get; set; }

        public DateTimeOffset? CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
