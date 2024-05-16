using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using backend.Models;

namespace backend.DTOs
{
    public class OrderInputDTO
    { 
        public long OrderInfoId {get;set;}

        public string Code {get;set;}
        public string? Note {get;set;}
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PaymentType PaymentType {get;set;}
        public List<OrderDetailInputDTO> OrderDetails {get;set;}

    }


    public class OrderGetDTO{
         public long Id {get;set;}
        public  UserGetShortDTO User {get;set;}
        public string Code {get;set;}
        public string? Note {get;set;}
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PaymentType PaymentType {get;set;}
        public  OrderInfo OrderInfo {get;set;}
        public  UserGetShortDTO UpdatedBy { get; set; }
        public int Status {get;set;}
        
        public DateTime? CreatedAt {get;set;}
        public DateTime? UpdatedAt {get;set;}
        public virtual List<OrderDetail>? OrderDetails {get;set;}


    }
        public class OrderDetailInputDTO
    { 
        public long ProductId {get;set;}
        public int Quantity {get;set;}

    }
    
    
}
