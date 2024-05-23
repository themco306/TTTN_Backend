using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using backend.Models;

namespace backend.DTOs
{
    public class PostInputDTO
    {
        public string Name { get; set; } 
        public long TopicId {get;set;}
        public IFormFile? Image { get; set; }
        public string Detail {get;set;}
         public int Status { get; set; }
    }
        public class PostGetShowDTO
    {
         public long Id { get; set; }
        public string Name { get; set; }
        public string Slug {get;set;}
        public long TopicId {get;set;}
        public Topic? Topic {get;set;}

        public string ImagePath { get; set; }
        public string Detail {get;set;}
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PostType Type {get;set;}
        public int Status { get; set; }
        public virtual UserGetShortDTO CreatedBy {get;set;}
        public virtual UserGetShortDTO UpdatedBy {get;set;}
                        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
    public class PostGetDTO
    {
         public long Id { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public Topic? Topic {get;set;}

        public string Slug {get;set;}
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }

    }
    
}
