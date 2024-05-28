using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using backend.Models;

namespace backend.DTOs
{
    public class OrderUpdateStatusDTO{
         [JsonConverter(typeof(JsonStringEnumConverter))]
        public OrderStatus Status { get; set; }
    }
    public class OrderInputDTO
    { 
        public long OrderInfoId {get;set;}

        public string Code {get;set;}
        public string? Note {get;set;}
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PaymentType PaymentType {get;set;}
        public List<OrderDetailInputDTO> OrderDetails {get;set;}

    }

public class OrderGetReceivedDTO{
         public long Id {get;set;}
        public  List<OrderDetail>? OrderDetails {get;set;}
    }
    public class OrderWithCouponDTO
{
    public OrderGetDTO Order { get; set; }
    public string CouponUsage { get; set; }
}

    public class OrderGetDTO{
         public long Id {get;set;}
        public  UserGetShortDTO User {get;set;}
        public string Code {get;set;}
        public decimal Total { get; set; }

        public string Token { get; set;}
        public string? Note {get;set;}
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PaymentType PaymentType {get;set;}
        public  OrderInfo OrderInfo {get;set;}
        public  UserGetShortDTO UpdatedBy { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public OrderStatus Status { get; set; }
        
        public DateTime? CreatedAt {get;set;}
        public DateTime? UpdatedAt {get;set;}
        public virtual List<OrderDetail>? OrderDetails {get;set;}

        public PaidOrder PaidOrder {get;set;}


    }
        public class OrderGetCreateDTO{
        public string Code {get;set;}
        public string Token {get;set;}
        public DateTime ExpiresAt {get;set;}


    }
        public class OrderDetailInputDTO
    { 
        public long CartId {get;set;}
        public long ProductId {get;set;}
        public int Quantity {get;set;}

    }
    
    
}
