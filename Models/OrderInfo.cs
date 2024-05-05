
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace backend.Models
{

    public class OrderInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id {get;set;}
        public string UserId {get;set;}
        public virtual  AppUser? User {get;set;}
        public string DeliveryAddress {get;set;}
        public string DeliveryName {get;set;}
        public string DeliveryPhone {get;set;}

    }
}