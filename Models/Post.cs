using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models
{
    public class Post :DateTimeInfo 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? TopicId {get;set;}
        [JsonIgnore]
        public virtual Topic? Topic {get;set;}
        public  string Name { get; set; }
        public string Slug {get;set;}
        public string Detail {get;set;}
        public string ImagePath { get; set; }
        public string Type {get;set;}
        public int Status {get;set;}=0;
        public string? CreatedById {get;set;}
        [JsonIgnore]
        public virtual AppUser? CreatedBy {get;set;}
        public string? UpdatedById {get;set;}
        [JsonIgnore]
        public virtual AppUser? UpdatedBy {get;set;}
        
    }
}
