
namespace backend.Models
{
    public class ProductCategory
    {
        public long ProductId {get;set;}
        public virtual Product? Product {get;set;}
        public long CategoryId {get;set;}
        public virtual Category? Category {get;set;}

       

    }
}