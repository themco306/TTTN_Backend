
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace backend.Models
{

    public class CartItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id {get;set;}
        public long CartId {get;set;}
        public virtual Cart? Cart {get;set;}
        public long ProductId {get;set;}
        public virtual Product? Product {get;set;}
        public int Quantity {get;set;}

    }
}