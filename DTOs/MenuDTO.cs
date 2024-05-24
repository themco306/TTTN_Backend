using System.ComponentModel.DataAnnotations;
using backend.Models;

namespace backend.DTOs
{
    public class MenuCustomInputDTO
    {
        public string Position { get; set; } 
        public string Name {get;set;}
        public string Link {get;set;}

    }
        public class MenuInputDTO
    {
        public string Position { get; set; } 
        public long TableId {get;set;}
        public string Type {get;set;}
    }
            public class MenuInputUpdateDTO
    {
        public string Position { get; set; } 
         public string Name {get;set;}
        public string Link {get;set;}
        public int SortOrder {get;set;}
        public long ParentId {get;set;}
        public int Status {get;set;}
    }
     public class MenuGetShortDTO{
         public long Id { get; set; }
         public string Name {get;set;}
         public string Link {get;set;}
     }
    public class MenuGetDTO{
         public long Id { get; set; }
         public string Name {get;set;}
         public string Link {get;set;}
         public string Type {get;set;}
         public long TableId {get;set;}
         public int SortOrder {get;set;}
         public string Position {get;set;}
         public  Menu Parent {get;set;}
        public  UserGetShortDTO CreatedBy {get;set;}
        public  UserGetShortDTO UpdatedBy {get;set;}
         public int Status {get;set;}
                 public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    
}
