using System;
using System.ComponentModel.DataAnnotations;
using backend.Models;

namespace backend.DTOs
{
        public class ProductSearchDTO
{
    public string? Query { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? SortBy { get; set; } = "updatedAt"; // default sorting field
    public string? SortOrder { get; set; } = "desc"; // default sorting order
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
    public class ProductFilterDTO
{
    public string? CategoryId { get; set; }
    public string? BrandId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? SortBy { get; set; } = "updatedAt"; // default sorting field
    public string? SortOrder { get; set; } = "desc"; // default sorting order
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
    public class ProductInputDTO
    {
        [Required(ErrorMessage = "Vui lòng chọn danh mục.")]
        public long CategoryId {get;set;}
                [Required(ErrorMessage = "Vui lòng chọn thương hiệu.")]
        public long BrandId {get;set;}
        [Required(ErrorMessage = "Vui lòng nhập tên.")]
        // [StringLength(200, ErrorMessage = "Tên không được vượt quá 200 ký tự.")]
        public string Name { get; set; }

        // [StringLength(255, ErrorMessage = "Mô tả không được vượt quá 255 ký tự.")]
        public string Description { get; set; }

        // [StringLength(255, ErrorMessage = "Chi tiết không được vượt quá 255 ký tự.")]
        public string Detail { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá bán.")]
        public decimal SalePrice { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá so sánh.")]
        public decimal ComparePrice { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập giá mua.")]
        public decimal? BuyingPrice { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập loại sản phẩm.")]
        [StringLength(64, ErrorMessage = "Loại sản phẩm không được vượt quá 64 ký tự.")]
        public string ProductType { get; set; }

        public string Note { get; set; }
        public int Status { get; set; }

        [Required(ErrorMessage = "Thiếu CreatedById")]
        public string CreatedById { get; set; }
        [Required(ErrorMessage = "Thiếu UpdatedById")]
        public string UpdatedById { get; set; }
    }

    public class ProductGetDTO
    {
        public long Id { get; set; }

        public Category Category {get;set;}
        public Brand Brand {get;set;}
        public string Name { get; set; }
        public string Slug { get; set; }

        public List<Gallery> Galleries { get; set; } = new List<Gallery>();
        public string Description { get; set; }
        public string Detail { get; set; }
        public decimal SalePrice { get; set; }
        public decimal ComparePrice { get; set; }
        public decimal? BuyingPrice { get; set; }
        public int Quantity { get; set; }
        public string ProductType { get; set; }
        public string Note { get; set; }
        public int Status { get; set; }
        public AppUser CreatedBy { get; set; }
        public AppUser UpdatedBy { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
