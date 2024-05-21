using System.ComponentModel.DataAnnotations;

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

    
}
