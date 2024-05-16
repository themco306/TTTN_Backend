

using backend.Models;

namespace backend.DTOs
{
    public class CartInputDTO
    {
        public CartItemInputDTO CartItem {get;set;}
    }
    public class CartItemInputDTO
    {
        public long ProductId {get;set;}
        public int Quantity {get;set;}=1;
    }
}