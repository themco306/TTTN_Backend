
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace backend.Models
{

    public class Order:DateTimeInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id {get;set;}
        public string UserId {get;set;}
        public virtual AppUser? User {get;set;}

        public string Code {get;set;}

        public long OrderInfoId {get;set;}
        public virtual OrderInfo OrderInfo {get;set;}

        public string? UpdatedById { get; set; }
        public virtual AppUser? UpdatedBy { get; set; }
        public int Status {get;set;}

        public virtual List<OrderDetail>? OrderDetails {get;set;}


    }
}