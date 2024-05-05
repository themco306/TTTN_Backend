using AutoMapper;
using backend.DTOs;
using backend.Exceptions;
using backend.Helper;
using backend.Models;
using backend.Repositories.IRepositories;


namespace backend.Services
{
    public class SliderService
    {
        private readonly ISilderRepository _sliderRepository;
        private readonly GalleryService _galleryService;
        private readonly Generate _generate;
        private readonly IMapper _mapper;

        public SliderService(ISilderRepository sliderRepository, GalleryService galleryService, Generate generate,IMapper mapper)
        {
            _sliderRepository = sliderRepository;
            _galleryService = galleryService;
            _generate = generate;
            _mapper=mapper;
        }

        public async Task<List<SliderGetDTO>> GetSlidersAsync()
        {
            var sliders = await _sliderRepository.GetAllAsync();

            return _mapper.Map<List<SliderGetDTO>>(sliders);
        }

        public async Task<SliderGetDTO> GetSliderByIdAsync(long sliderId)
        {
            var slider = await _sliderRepository.GetByIdAsync(sliderId);
            if (slider == null)
            {
                throw new NotFoundException("Hình ảnh không tồn tại");
            }

            return _mapper.Map<SliderGetDTO>(slider);
        }
                public async Task<Slider> GetSliderShowByIdAsync(long sliderId)
        {
            var slider = await _sliderRepository.GetByIdAsync(sliderId);
            if (slider == null)
            {
                throw new NotFoundException("Hình ảnh không tồn tại");
            }

            return slider;
        }

        public async Task<SliderGetDTO> CreateSliderAsync(SliderInputDTO sliderDTO)
        {
            var slugName = _generate.GenerateSlug(sliderDTO.Name);
            var slider = new Slider
            {
                Name = sliderDTO.Name,
                ImagePath = await _galleryService.UploadImage(slugName, sliderDTO.Image, "sliders"),
                Status = 0,
                CreatedById=sliderDTO.CreatedById,
                UpdatedById=sliderDTO.CreatedById
            };
            await _sliderRepository.AddAsync(slider);
            return _mapper.Map<SliderGetDTO>(slider);
        }

        public async Task<SliderGetDTO> UpdateSliderAsync(long sliderId, SliderInputDTO sliderDTO)
        {
            var existingSlider = await _sliderRepository.GetByIdAsync(sliderId);
            if (existingSlider == null)
            {
                throw new NotFoundException("Hình ảnh không tồn tại");
            }

            existingSlider.Name = sliderDTO.Name;
            existingSlider.UpdatedById=sliderDTO.UpdatedById;
            if (sliderDTO.Image != null)
            {
                var slugName = _generate.GenerateSlug(sliderDTO.Name);
                await _galleryService.DeleteImageAsync(existingSlider.ImagePath, "sliders");
                existingSlider.ImagePath = await _galleryService.UploadImage(slugName, sliderDTO.Image, "sliders");
            }
            await _sliderRepository.UpdateAsync(existingSlider);

                        return _mapper.Map<SliderGetDTO>(existingSlider);

        }

        public async Task DeleteSliderAsync(long sliderId)
        {
            var existingSlider = await _sliderRepository.GetByIdAsync(sliderId);
            if (existingSlider == null)
            {
                throw new NotFoundException("Hình ảnh không tồn tại");
            }
            await _galleryService.DeleteImageAsync(existingSlider.ImagePath, "sliders");
            await _sliderRepository.DeleteAsync(existingSlider);
        }
        public async Task DeleteSlidersAsync(List<long> ids)
        {
            foreach (var id in ids)
            {
                var existingCategory = await _sliderRepository.GetByIdAsync(id);
                if (existingCategory != null)
                {
                     await _galleryService.DeleteImageAsync(existingCategory.ImagePath, "sliders");
                    await _sliderRepository.DeleteAsync(existingCategory);
                }
            }
        }
        public async Task UpdateSliderStatusAsync(long id)
        {
            var existing = await _sliderRepository.GetByIdAsync(id);

            if (existing == null)
            {
                throw new NotFoundException("Danh mục không tồn tại");
            }
            existing.Status = existing.Status == 0 ? 1 : 0;

            await _sliderRepository.UpdateAsync(existing);
        }
    }
}
