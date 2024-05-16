
using AutoMapper;
using backend.DTOs;
using backend.Exceptions;
using backend.Helper;
using backend.Models;
using backend.Repositories.IRepositories;


namespace backend.Services
{
    public class WebInfoService
    {
        private readonly IWebInfoRepository _webInfoRepository;
        private readonly GalleryService _galleryService;
        private readonly Generate _generate;

        public WebInfoService(IWebInfoRepository webInfoRepository,GalleryService galleryService,Generate generate)
        {
            _webInfoRepository = webInfoRepository;
            _galleryService=galleryService;
            _generate=generate;

        }
        public async Task<WebInfo> GetFirstWebInfoAsync()
        {
            var webInfo = await _webInfoRepository.GetFirstAsync();
            return webInfo;
        }
        public async Task<WebInfo> UpdateWebInfoAsync(long id, WebInfoInputDTO webInfo)
        {
            var existingWebInfo = await _webInfoRepository.GetByIdAsync(id);
            if (existingWebInfo == null)
            {
                throw new NotFoundException("Thông tin không tồn tại.");
            }
            if(webInfo.Icon!=null){
                 if (existingWebInfo.Icon != "default_icon.png")
                {
                    await _galleryService.DeleteImageAsync(existingWebInfo.Icon, "logo");
                }
                var slugName= _generate.GenerateSlug(webInfo.ShopName)??"mac-dinh";
                var AvatarUpload = await _galleryService.UploadImage(slugName, webInfo.Icon, "logo");
                existingWebInfo.Icon = AvatarUpload;
            }
            existingWebInfo.ShopName = webInfo.ShopName;
            existingWebInfo.Description = webInfo.Description;
            existingWebInfo.PhoneNumber = webInfo.PhoneNumber;
            existingWebInfo.Email = webInfo.Email;
            existingWebInfo.Address = webInfo.Address;
            existingWebInfo.GoogleMap = webInfo.GoogleMap;
            existingWebInfo.WorkingHours = webInfo.WorkingHours;
            existingWebInfo.FacebookLink = webInfo.FacebookLink;
            existingWebInfo.InstagramLink = webInfo.InstagramLink;
            existingWebInfo.TwitterLink = webInfo.TwitterLink;
            await _webInfoRepository.UpdateAsync(existingWebInfo);
            return existingWebInfo;
        }


    }
}
