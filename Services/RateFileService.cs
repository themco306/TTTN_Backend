using backend.DTOs;
using backend.Exceptions;
using backend.Models;
using backend.Repositories.IRepositories;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace backend.Services
{
    public class RateFileService
    {
        private readonly IRateFileRepository _rateFileRepository;
        private readonly IRateRepository _rateRepository;

        public RateFileService(IRateFileRepository rateFileRepository, IRateRepository rateRepository)
        {
            _rateFileRepository = rateFileRepository;
            _rateRepository = rateRepository;
        }

        public async Task<List<RateFile>> GetRateFilesByRateIdAsync(long rateId)
        {
            var rateFiles = await _rateFileRepository.GetRateFilesByRateIdAsync(rateId);
            return rateFiles;
        }

        public async Task<List<RateFile>> CreateRateFilesAsync(long rateId, List<IFormFile> newFiles)
        {
            var rate = await _rateRepository.GetByIdAsync(rateId);
            if (rate == null)
            {
                throw new NotFoundException("Đánh giá không tồn tại.");
            }

            if (newFiles == null || newFiles.Count == 0)
            {
                throw new Exception("Vui lòng chọn ít nhất một file.");
            }

            var rateFiles = new List<RateFile>();

            foreach (var file in newFiles)
            {
                var fileType = GetFileType(file);
                var uploadedFileName = await UploadFile(rateId, file, fileType);

                var rateFile = new RateFile
                {
                    RateId = rateId,
                    FilePath = uploadedFileName,
                    FileType = fileType
                };

                await _rateFileRepository.AddAsync(rateFile);
                rateFiles.Add(rateFile);
            }

            return rateFiles;
        }
public async Task UpdateRateFilesAsync(long rateId, List<long> ids, List<IFormFile> newFiles)
{
            var rate = await _rateRepository.GetByIdAsync(rateId);
            if (rate == null)
            {
                throw new NotFoundException("Đánh giá không tồn tại.");
            }

    var currentImages = await _rateFileRepository.GetRateFilesByRateIdAsync(rateId);
    var imagesToRemove = currentImages.Where(img => !ids.Contains(img.Id)).ToList();
    if (imagesToRemove.Count > 0)
    {
        foreach (var image in imagesToRemove)
        {
            await DeleteRateFileAsync(image.Id);
        }
    }

    if (newFiles.Count > 0)
    {
        await CreateRateFilesAsync(rateId, newFiles);
    }
}
        private async Task<string> UploadFile(long rateId, IFormFile file, string fileType)
        {
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/uploads/{fileType}s");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var fileExtension = Path.GetExtension(file.FileName);
            var fileName = $"{rateId}_{DateTime.Now.Ticks}{fileExtension}";
            var filePath = Path.Combine(uploadPath, fileName);

            if (fileType == "image")
            {
                using (var image = await Image.LoadAsync(file.OpenReadStream()))
                {
                    await image.SaveAsJpegAsync(filePath);
                }
            }
            else if (fileType == "video")
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }

            return fileName;
        }

        private string GetFileType(IFormFile file)
        {
            var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var videoExtensions = new[] { ".mp4", ".avi", ".mov", ".wmv" };

            var extension = Path.GetExtension(file.FileName).ToLower();

            if (imageExtensions.Contains(extension))
            {
                return "image";
            }
            else if (videoExtensions.Contains(extension))
            {
                return "video";
            }
            else
            {
                throw new Exception("Loại tệp không hợp lệ.");
            }
        }

        public async Task DeleteRateFileAsync(long id)
        {
            var existingRateFile = await _rateFileRepository.GetByIdAsync(id);
            if (existingRateFile == null)
            {
                throw new NotFoundException("Tệp đánh giá không tồn tại.");
            }
            await DeleteFileAsync(existingRateFile.FilePath,existingRateFile.FileType);
            await _rateFileRepository.DeleteAsync(id);
        }

        private async Task DeleteFileAsync(string filePath,string fileType)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/"+fileType+"s/", filePath);

            if (File.Exists(path))
            {
                File.Delete(path);
            }
            else
            {
                throw new FileNotFoundException("Không tìm thấy tệp cần xóa.");
            }
        }
    }
}
