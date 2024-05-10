using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models
{
    public enum TagType
    {
        NewModel,
        BestSeller,
        // Thêm các loại tag khác nếu cần
    }
    
    public class Tag 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Name { get; set; }
        public TagType Type {get;set;}
        public string Description { get; set; }
        public long Sort {get;set;}
        public List<ProductTag> ProductTags { get; set; }
        
    }
}
