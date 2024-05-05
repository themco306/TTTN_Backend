using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public class SliderInputDTO
    {
        public string Name { get; set; } 

        public IFormFile? Image { get; set; }

               [Required(ErrorMessage = "Thiếu CreatedById")]
        public string CreatedById { get; set; }
        [Required(ErrorMessage = "Thiếu UpdatedById")]
        public string UpdatedById { get; set; }

    }
    
    public class SliderGetDTO
    {
         public long Id { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public int Status { get; set; }
        public string? CreatedById {get;set;}
        public string? UpdatedById {get;set;}
        

    }
    
}
