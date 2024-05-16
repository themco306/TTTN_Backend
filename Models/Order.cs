
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace backend.Models
{
    public enum PaymentType {
    CashOnDelivery, // Thanh toán khi nhận hàng
    OnlinePayment // Thanh toán trực tuyến
}

    public class Order:DateTimeInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id {get;set;}
        public string UserId {get;set;}
        public virtual AppUser? User {get;set;}

        public string Code {get;set;}
        public string? Note {get;set;}
        public decimal Total { get; set; }
        public string Token {get;set;}
        public DateTime ExpiresAt {get;set;}

        public long OrderInfoId {get;set;}
        public virtual OrderInfo OrderInfo {get;set;}

        public string? UpdatedById { get; set; }
        public virtual AppUser? UpdatedBy { get; set; }
         
        public PaymentType PaymentType {get;set;}=PaymentType.CashOnDelivery;
        public int Status {get;set;}
        public virtual List<OrderDetail>? OrderDetails {get;set;}


    }
}