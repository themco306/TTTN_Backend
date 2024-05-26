
using AutoMapper;
using backend.DTOs;
using backend.Exceptions;
using backend.Helper;
using backend.Models;
using backend.Repositories.IRepositories;


namespace backend.Services
{
    public class BrandService
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IMapper _mapper;
        private readonly Generate _generate;
        private readonly AccountService _accountService;

        public BrandService(IBrandRepository brandRepository, IMapper mapper, Generate generate, AccountService accountService)
        {
            _brandRepository = brandRepository;
            _mapper = mapper;
            _generate = generate;
            _accountService = accountService;
        }
        public async Task<List<BrandGetDTO>> GetAllBrandsAsync()
        {
            var brands = await _brandRepository.GetAllAsync();
            return _mapper.Map<List<BrandGetDTO>>(brands);
        }
       public async Task<List<BrandGetDTO>> GetAllBrandsActiveAsync()
        {
            var brands = await _brandRepository.GetAllActiveAsync();
            return _mapper.Map<List<BrandGetDTO>>(brands);
        }
        public async Task<BrandGetDTO> GetBrandByIdAsync(long id)
        {
            var brand = await _brandRepository.GetByIdAsync(id);
            if (brand == null)
            {
                throw new NotFoundException("Danh mục không tồn tại.");
            }
            return _mapper.Map<BrandGetDTO>(brand);
        }
        public async Task<BrandGetDTO> CreateBrandAsync(BrandInputDTO brandInputDTO,string token)
        {
            var userId = _accountService.ExtractUserIdFromToken(token);
            if (userId == null)
            {
                throw new NotFoundException("Có lỗi xãy ra vui lòng đăng nhập lại");
            }
            var user = await _accountService.GetUserByIdAsync(userId);
            string slug = _generate.GenerateSlug(brandInputDTO.Name);

            var brand = new Brand{
                Name = brandInputDTO.Name,
                Slug=slug,
                Status=brandInputDTO.Status,
                CreatedById=user.Id,
                UpdatedById=user.Id
            };
            await _brandRepository.AddAsync(brand);
            return _mapper.Map<BrandGetDTO>(brand);
        }
        public async Task<BrandGetDTO> UpdateBrandAsync(long id, BrandInputDTO brandInputDTO,string token)
        {
             var userId = _accountService.ExtractUserIdFromToken(token);
            if (userId == null)
            {
                throw new NotFoundException("Có lỗi xãy ra vui lòng đăng nhập lại");
            }
            var user = await _accountService.GetUserByIdAsync(userId);
            var existingBrand = await _brandRepository.GetByIdAsync(id);

            if (existingBrand == null)
            {
                throw new NotFoundException("Thương hiệu không tồn tại");
            }
            string slug = _generate.GenerateSlug(brandInputDTO.Name);
            existingBrand.Name=brandInputDTO.Name;
            existingBrand.Slug=slug;
            existingBrand.Status=brandInputDTO.Status;
            existingBrand.UpdatedById=user.Id;
            await _brandRepository.UpdateAsync(existingBrand);
                        return _mapper.Map<BrandGetDTO>(existingBrand);

        }
        public async Task DeleteBrandAsync(long id)
        {
            var existingBrand = await _brandRepository.GetByIdAsync(id);
            if (existingBrand == null)
            {
                throw new NotFoundException("Thương hiệu không tồn tại.");
            }


            await _brandRepository.DeleteAsync(id);
        }
        public async Task DeleteBrandsAsync(List<long> ids)
        {
            foreach (var id in ids)
            {
                var existingBrand = await _brandRepository.GetByIdAsync(id);
                if (existingBrand != null)
                {
                    await _brandRepository.DeleteAsync(id);
                }
            }
        }
public async Task UpdateBrandStatusAsync(long id)
{
    var existingBrand = await _brandRepository.GetByIdAsync(id);

    if (existingBrand == null)
    {
        throw new NotFoundException("Thương hiệu không tồn tại");
    }

    // Cập nhật trạng thái mới (nếu hiện tại là 0 thì cập nhật thành 1, và ngược lại)
    existingBrand.Status = existingBrand.Status == 0 ? 1 : 0;

    await _brandRepository.UpdateAsync(existingBrand);
}


    }
}
