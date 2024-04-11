using backend.DTOs;
using backend.Exceptions;
using backend.Models;
using backend.Repositories.IRepositories;
using Microsoft.AspNetCore.Http.HttpResults;
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
        public async Task<List<Gallery>> CreateGalleryAsync(GalleryInputDTO galleryInputDTO)
        {
            var product = await _productRepository.GetByIdAsync(galleryInputDTO.ProductId);
            if (product == null)
            {
                throw new NotFoundException("Sản phẩm không tồn tại.");
            }

            var uploadedImages = galleryInputDTO.Images; // Sửa lại tên property Images thay vì Image

            if (uploadedImages == null || uploadedImages.Count == 0)
            {
                throw new Exception("Vui lòng chọn ít nhất một file ảnh.");
            }

            var galleries = new List<Gallery>();

            int order = 1;
            foreach (var image in uploadedImages)
            {
                var uploadedImageNames = await UploadImages(product.Slug, image);
                foreach (var imageName in uploadedImageNames)
                {
                    var gallery = new Gallery
                    {
                        ProductId = galleryInputDTO.ProductId,
                        ImageName = imageName,
                        ImagePath = $"/images/products/{imageName}",
                        Placeholder = "productdefaultimage.jpg",
                        Order = order // Gán order cho mỗi ảnh
                    };

                    await _galleryRepository.AddAsync(gallery);
                    galleries.Add(gallery);
                    order++; // Tăng order sau mỗi lần tạo gallery
                }
            }

            return galleries;
        }


        public async Task<List<string>> UploadImages(string productSlug, IFormFile imageFile)
        {
            var uploadedFileNames = new List<string>();
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/images/products");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            var fileName = $"{productSlug}_{DateTime.Now.Ticks}.jpg";
            var filePath = Path.Combine(uploadPath, fileName);
            using (var image = await Image.LoadAsync(imageFile.OpenReadStream()))
            {
                await image.SaveAsJpegAsync(filePath);
            }
            uploadedFileNames.Add(fileName);

            return uploadedFileNames;
        }

        public async Task UpdateGalleryImagesAsync(long productId, List<IFormFile> newImages)
        {
            var existingProduct=await _productRepository.GetByIdAsync(productId);
            if(existingProduct==null){
                throw new NotFoundException("Sản phẩm không tồn tại");
            }
            var currentImages = await _galleryRepository.GetGalleriesByProductIdAsync(productId);

            // Thêm ảnh mới
            foreach (var image in newImages)
            {
                var uploadedImageNames = await UploadImages(existingProduct.Slug, image);
                foreach (var imageName in uploadedImageNames)
                {
                    var gallery = new Gallery
                    {
                        ProductId = productId,
                        ImageName = imageName,
                        ImagePath = $"/images/products/{imageName}",
                        Placeholder = "productdefaultimage.jpg",
                        Order = currentImages.Count + 1 // Thêm ảnh mới vào cuối danh sách
                    };

                    await _galleryRepository.AddAsync(gallery);
                }
            }

            // Xóa các ảnh không còn được sử dụng
            foreach (var currentImage in currentImages)
            {
                // Kiểm tra xem ảnh hiện tại có trong danh sách ảnh mới không
                var isUsed = newImages.Any(newImage => newImage.FileName == currentImage.ImageName);

                // Nếu không tìm thấy trong danh sách ảnh mới, xóa ảnh hiện tại
                if (!isUsed)
                {
                    await _galleryRepository.DeleteAsync(currentImage.Id);
                    // Xóa file ảnh từ thư mục nếu cần
                    await DeleteImageAsync(currentImage.ImageName);
                }
            }
        }



        // public async Task UpdateGalleryAsync(long id, GalleryInputDTO galleryInputDTO)
        // {
        //     var existingGallery = await _galleryRepository.GetByIdAsync(id);
        //     if (existingGallery == null)
        //     {
        //         throw new NotFoundException("Bộ sưu tập hình ảnh không tồn tại.");
        //     }

        //     existingGallery.ProductId = galleryInputDTO.ProductId;
        //     existingGallery.Placeholder = "productdefaultimage.jpg";
        //     existingGallery.Order = galleryInputDTO.Order;

        //     await _galleryRepository.UpdateAsync(existingGallery);
        // }

        public async Task DeleteGalleryAsync(long id)
        {
            var existingGallery = await _galleryRepository.GetByIdAsync(id);
            if (existingGallery == null)
            {
                throw new NotFoundException("Bộ sưu tập hình ảnh không tồn tại.");
            }

            await _galleryRepository.DeleteAsync(id);
        }

        public async Task DeleteImageAsync(string imageName)
        {
            // Kiểm tra nếu tên hình ảnh không hợp lệ
            if (string.IsNullOrEmpty(imageName))
            {
                throw new ArgumentException("Tên hình ảnh không hợp lệ.");
            }

            // Đường dẫn đến thư mục chứa hình ảnh
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products", imageName);

            // Kiểm tra nếu tệp tin hình ảnh tồn tại
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
