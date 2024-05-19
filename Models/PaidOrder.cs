using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models
{
    public enum PaymentMethod{
        MomoPayment
    } 
    public class PaidOrder 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long OrderId {get;set;}
        [JsonIgnore]
        public virtual Order Order {get;set;}

        public DateTime? PaymentDate {get;set;}

        public decimal Amount {get;set;}

        public PaymentMethod PaymentMethod {get;set;}

        public string PaymentMethodCode  {get;set;}
        
    }
}
