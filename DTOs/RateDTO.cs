

using System.ComponentModel.DataAnnotations;
using backend.Models;

namespace backend.DTOs
{
    public class RateInputDTO
    {

        public  int? Star { get; set; }=1;
        public string? Content {get;set;}
        public long? ProductId {get;set;}

    }
    public class RateLikeGetDTO{
         public long Id {get;set;}
        public  bool IsLike { get; set; }
    }
    public class RateGetDTO
    {
         public long Id {get;set;}
        public  int? Star { get; set; }
        public string? Content {get;set;}
        public int? Like {get;set;}=0;
        public int? Dislike {get;set;}=0;
        public int Status {get;set;}=0;
        public string? UserId {get;set;}

                public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public virtual UserGetShortDTO? User {get;set;}
        public long? ProductId {get;set;}

    }

}