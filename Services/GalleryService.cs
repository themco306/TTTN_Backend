using backend.DTOs;
using backend.Exceptions;
using backend.Models;
using backend.Repositories.IRepositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Mysqlx.Crud;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace backend.Services
{
    public class GalleryService
    {
        private readonly IGalleryRepository _galleryRepository;
        private readonly IProductRepository _productRepository;

        public GalleryService(IGalleryRepository galleryRepository, IProductRepository productRepository)
        {
            _galleryRepository = galleryRepository;
            _productRepository = productRepository;
        }
        public async Task<List<GalleryDTO>> GetGalleriesByProductIdAsync(long productId)
        {
            var galleries = await _galleryRepository.GetGalleriesByProductIdAsync(productId);

            // Chuyển đổi danh sách Gallery thành danh sách GalleryDTO để trả về
            var galleryDTOs = new List<GalleryDTO>();
            foreach (var gallery in galleries)
            {
                galleryDTOs.Add(new GalleryDTO
                {
                    Id = gallery.Id,
                    ProductId = gallery.ProductId,
                    ImagePath = gallery.ImagePath,
                    ImageName = gallery.ImageName,
                    Placeholder = gallery.Placeholder,
                    Order = gallery.Order,
                    CreatedAt = gallery.CreatedAt,
                    UpdatedAt = gallery.UpdatedAt
                });
            }

            return galleryDTOs;
        }
        public async Task<List<Gallery>> CreateGalleryAsync(long productId, List<IFormFile> newImages,int order=1)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                throw new NotFoundException("Sản phẩm không tồn tại.");
            }

            // var uploadedImages = newImages; // Sửa lại tên property Images thay vì Image

            if (newImages == null || newImages.Count == 0)
            {
                throw new Exception("Vui lòng chọn ít nhất một file ảnh.");
            }

            var galleries = new List<Gallery>();

            foreach (var image in newImages)
            {
                var uploadedImageName = await UploadImage(product.Slug, image);
                    var gallery = new Gallery
                    {
                        ProductId = productId,
                        ImageName = uploadedImageName,
                        ImagePath = $"/images/products/{uploadedImageName}",
                        Placeholder = "/images/defaultimage/productdefaultimage.jpg",
                        Order = order 
                    };

                    await _galleryRepository.AddAsync(gallery);
                    galleries.Add(gallery);
                    order++;
            }

            return galleries;
        }


public async Task<string> UploadImage(string productSlug, IFormFile imageFile, string type = "products")
{
    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/images/" + type);
    if (!Directory.Exists(uploadPath))
    {
        Directory.CreateDirectory(uploadPath);
    }
    var fileExtension = Path.GetExtension(imageFile.FileName); // Lấy đuôi của tệp ảnh
    var fileName = $"{productSlug}_{DateTime.Now.Ticks}{fileExtension}";
    var filePath = Path.Combine(uploadPath, fileName);
    using (var image = await Image.LoadAsync(imageFile.OpenReadStream()))
    {
        await image.SaveAsJpegAsync(filePath);
    }

    return fileName;
}

public async Task UpdateGalleryImagesAsync(long productId, List<long> ids, List<IFormFile> newImages)
{
    var existingProduct = await _productRepository.GetByIdAsync(productId);
    if (existingProduct == null)
    {
        throw new NotFoundException("Sản phẩm không tồn tại");
    }

    var currentImages = await _galleryRepository.GetGalleriesByProductIdAsync(productId);
    var maxOrder = currentImages.Max(img => img.Order); // Find the highest order number

    var imagesToRemove = currentImages.Where(img => !ids.Contains(img.Id)).ToList();
    if (imagesToRemove.Count > 0)
    {
        foreach (var image in imagesToRemove)
        {
            await DeleteGalleryAsync(image.Id);
        }
    }

    if (newImages.Count > 0)
    {
        // Set the order for the new images
        var newOrder = maxOrder + 1;

        // Create the new galleries with the updated order
        var galleries = await CreateGalleryAsync(productId, newImages, newOrder);
    }
}

         public async Task DeleteGalleryByProductIdAsync(long id)
        {
            var products = await GetGalleriesByProductIdAsync(id);
            if(products!=null){
                foreach(var product  in products){
                    await DeleteGalleryAsync( product.Id);
                }
            }
        }

        public async Task DeleteGalleryAsync(long id)
        {
            var existingGallery = await _galleryRepository.GetByIdAsync(id);
            if (existingGallery == null)
            {
                throw new NotFoundException("Bộ sưu tập hình ảnh không tồn tại.");
            }
            await DeleteImageAsync(existingGallery.ImageName);
            await _galleryRepository.DeleteAsync(id);
        }

        public async Task DeleteImageAsync(string imageName,string type="products")
        {
            // Kiểm tra nếu tên hình ảnh không hợp lệ
            if (string.IsNullOrEmpty(imageName))
            {
                return;
            }

            // Đường dẫn đến thư mục chứa hình ảnh
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/"+type, imageName);

            // Kiểm tra nếu tệp tin hình ảnh tồn tạidot
            if (File.Exists(imagePath))
            {
                // Xóa hình ảnh khỏi thư mục
                File.Delete(imagePath);
            }
            else
            {
                // Nếu không tìm thấy tệp tin hình ảnh, ném một ngoại lệ
                throw new FileNotFoundException("Không tìm thấy hình ảnh cần xóa.");
            }
        }
    }
}
