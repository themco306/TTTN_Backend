

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
}