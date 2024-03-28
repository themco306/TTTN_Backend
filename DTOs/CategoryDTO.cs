

namespace backend.DTOs
{
    public class CategoryInputDTO
    {
        public long? ParentId {get;set;} =null;
        public  string Name { get; set; }

        public  string Slug { get; set; }

        public string Description {get;set;}

        public string Image {get;set;}

        public string Icon {get;set;}

        public long CreatedBy {get;set;}

        public long UpdatedBy {get;set;}
    }

        public class CategoryGetDTO
    {
        public long Id {get;set;}
        public long? ParentId {get;set;} =null;
        public  string Name { get; set; }

        public  string Slug { get; set; }

        public string Description {get;set;}

        public string Image {get;set;}

        public string Icon {get;set;}

        public long CreatedBy {get;set;}

        public long UpdatedBy {get;set;}

    }

}