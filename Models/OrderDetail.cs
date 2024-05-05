
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace backend.Models
{

    public class OrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id {get;set;}
        public long OrderId {get;set;}
        [JsonIgnore]
        public virtual Order Order {get;set;}
        public long ProductId {get;set;}
        public virtual Product Product {get; set;}
        public int Quantity {get;set;}
        public decimal Price {get;set;} 
        public decimal TotalPrice {get;set;}

    }
}