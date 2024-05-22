using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using backend.Models;

namespace backend.DTOs
{
    public class PostInputDTO
    {
        public string Name { get; set; } 
        public long TopicId {get;set;}=0;
        public IFormFile? Image { get; set; }
        public string Detail {get;set;}
         public int Status { get; set; }
    }
    
    public class PostGetDTO
    {
         public long Id { get; set; }
        public string Name { get; set; }
        public long TopicId {get;set;}
        public string ImagePath { get; set; }
        public string Detail {get;set;}
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PostType Type {get;set;}
        public int Status { get; set; }
        public string? CreatedById {get;set;}
        public string? UpdatedById {get;set;}
        

    }
    
}
