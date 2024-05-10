using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models
{
    public class ProductTag 
    {
         public long ProductId { get; set; }
        public Product Product { get; set; }

        public long TagId { get; set; }
        public Tag Tag { get; set; }
        
    }
}
