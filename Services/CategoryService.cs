
using AutoMapper;
using backend.DTOs;
using backend.Exceptions;
using backend.Helper;
using backend.Models;
using backend.Repositories.IRepositories;


namespace backend.Services
{
    public class CategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly Generate _generate;
        private readonly AccountService _accountService;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper, Generate generate, AccountService accountService)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _generate = generate;
            _accountService = accountService;
        }
        public async Task<List<CategoryGetDTO>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return _mapper.Map<List<CategoryGetDTO>>(categories);
        }
        public async Task<List<CategoryGetDTO>> GetAllCategoriesActiveAsync()
        {
            var categories = await _categoryRepository.GetAllActiveAsync();
            return _mapper.Map<List<CategoryGetDTO>>(categories);
        }
        public async Task<List<CategoryGetDTO>> GetParentCategoriesAsync(long id)
        {
            var currentCategory = await _categoryRepository.GetByIdAsync(id);
            if (currentCategory == null)
            {
                throw new NotFoundException("Danh mục không tồn tại.");
            }

            List<Category> parentCategories;
            // Nếu danh mục hiện tại là một danh mục gốc
            parentCategories = await _categoryRepository.GetAllAsync(); // Lấy tất cả danh mục



            // Loại bỏ danh mục hiện tại và tất cả các danh mục con của nó khỏi danh sách danh mục cha
            parentCategories = parentCategories.Where(c => c.Id != id && !IsDescendant(currentCategory, c)).ToList();

            return _mapper.Map<List<CategoryGetDTO>>(parentCategories);
        }

        private bool IsDescendant(Category parent, Category child)
        {
            // Kiểm tra xem child có phải là một con của parent không
            if (child.ParentId == null)
            {
                return false;
            }
            if (child.ParentId == parent.Id)
            {
                return true;
            }
            return IsDescendant(parent, child.Parent); // Đệ quy kiểm tra các danh mục cha của child
        }

        public async Task<List<CategoryGetDTO>> GetChildByParentIdAsync(long id)
        {
            var existingParent = await _categoryRepository.GetByIdAsync(id);
            if (existingParent == null)
            {
                throw new NotFoundException("Danh mục Cha không tồn tại.");
            }
            var categories = await _categoryRepository.GetChildCategoriesAsync(id);
            return _mapper.Map<List<CategoryGetDTO>>(categories);
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
            if (categoryInputDTO.ParentId.HasValue)
            {
                var existingParent = await _categoryRepository.GetByIdAsync(categoryInputDTO.ParentId.Value);
                if (existingParent == null)
                {
                    throw new NotFoundException("Danh mục cha không tồn tại.");
                }
            }
            await _accountService.CUExistingUser(categoryInputDTO.CreatedById, categoryInputDTO.UpdatedById);
            // Tạo slug từ Name
            string slug = _generate.GenerateSlug(categoryInputDTO.Name);

            // Ánh xạ CategoryInputDTO sang Category
            var category = _mapper.Map<Category>(categoryInputDTO);

            // Gán giá trị slug cho trường Slug của Category
            category.Slug = slug;
            await _categoryRepository.AddAsync(category);
            return category;
        }
        public async Task<Category> UpdateCategoryAsync(long id, CategoryInputDTO categoryInputDTO)
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(id);

            if (existingCategory == null)
            {
                throw new NotFoundException("Danh mục không tồn tại");
            }

            if (categoryInputDTO.ParentId.HasValue)
            {
                var existingParent = await _categoryRepository.GetByIdAsync(categoryInputDTO.ParentId.Value);
                if (existingParent == null)
                {
                    throw new NotFoundException("Danh mục cha không tồn tại.");
                }
            }

            await _accountService.CUExistingUser(categoryInputDTO.CreatedById, categoryInputDTO.UpdatedById);

            // Tạo slug từ Name
            string slug = _generate.GenerateSlug(categoryInputDTO.Name);

            // Cập nhật các thuộc tính từ categoryInputDTO sang existingCategory
            _mapper.Map(categoryInputDTO, existingCategory);

            // Gán giá trị slug và CreatedById
            existingCategory.Slug = slug;
            existingCategory.CreatedById = categoryInputDTO.CreatedById;

            // Cập nhật category trong cơ sở dữ liệu
            await _categoryRepository.UpdateAsync(existingCategory);
            return existingCategory;
        }
        public async Task DeleteCategoryAsync(long id)
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(id);
            if (existingCategory == null)
            {
                throw new NotFoundException("Danh mục không tồn tại.");
            }


            await _categoryRepository.DeleteAsync(id);
        }
        public async Task DeleteCategoriesAsync(List<long> ids)
        {
            foreach (var id in ids)
            {
                var existingCategory = await _categoryRepository.GetByIdAsync(id);
                if (existingCategory != null)
                {
                    await _categoryRepository.DeleteAsync(id);
                }
            }
        }
public async Task<Category> UpdateCategoryStatusAsync(long id)
{
    var existingCategory = await _categoryRepository.GetByIdAsync(id);

    if (existingCategory == null)
    {
        throw new NotFoundException("Danh mục không tồn tại");
    }

    // Cập nhật trạng thái mới (nếu hiện tại là 0 thì cập nhật thành 1, và ngược lại)
    existingCategory.Status = existingCategory.Status == 0 ? 1 : 0;

    await _categoryRepository.UpdateAsync(existingCategory);

    return existingCategory;
}


    }
}
