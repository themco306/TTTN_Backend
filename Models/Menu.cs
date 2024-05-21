using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models
{
    public class Menu :DateTimeInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
         public string Name {get;set;}
         public string Link {get;set;}
         public string Type {get;set;}
         public long TableId {get;set;}=0;
         public int SortOrder {get;set;}=1;
         public string Position {get;set;}
         public long? ParentId {get;set;}
         public virtual Menu? Parent {get;set;}
        public string? CreatedById {get;set;}
        [JsonIgnore]
        public virtual AppUser? CreatedBy {get;set;}
        public string? UpdatedById {get;set;}
        [JsonIgnore]
        public virtual AppUser? UpdatedBy {get;set;}
         public int Status {get;set;}=0;

       
    }
}
