
using AutoMapper;
using backend.DTOs;
using backend.Exceptions;
using backend.Models;
using backend.Repositories.IRepositories;


namespace backend.Services
{
    public class CategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }
        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _categoryRepository.GetAllAsync();
        }
        public async Task<Category> GetCategoryByIdAsync(long id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                throw new NotFoundException("Danh mục không tồn tại.");
            }
            return category;
        }
        public async Task<Category> CreateCategoryAsync(CategoryInputDTO categoryInputDTO)
        {
            var category = _mapper.Map<Category>(categoryInputDTO);
            await _categoryRepository.AddAsync(category);
            return category;
        }

        public async Task UpdateCategoryAsync(long id, CategoryInputDTO categoryInputDTO)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
            {
                throw new NotFoundException("Danh mục không tồn tại");
            }

            _mapper.Map(categoryInputDTO, category);

            await _categoryRepository.UpdateAsync(category);
        }

        public async Task DeleteCategoryAsync(long id)
        {
            await _categoryRepository.DeleteAsync(id);
        }
    }
}
