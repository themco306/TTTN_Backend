

namespace backend.DTOs
{
    public class TopicInputDTO
    { 
        public long ParentId {get;set;}
        public  string Name { get; set; }
        public int Status {get;set;}
    }
        public class TopicUpdateDTO
    { 
        public long ParentId {get;set;}
        public  string Name { get; set; }
        public int SortOrder {get;set;}
        public int Status {get;set;}
    }
        public class TopicGetDTO
    { 
      public long Id { get; set; }
        public long ParentId {get;set;}
        public  string Name { get; set; }
        public string Slug {get;set;}
        public int SortOrder {get;set;}
        public int Status {get;set;}
        public virtual UserGetShortDTO CreatedBy {get;set;}
        public virtual UserGetShortDTO UpdatedBy {get;set;}
                public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}