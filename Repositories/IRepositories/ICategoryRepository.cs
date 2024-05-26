
using backend.Models;

namespace backend.Repositories.IRepositories
{
    public interface ICategoryRepository
    {
        Task<Category> GetByIdAsync(long id);
        Task<List<Category>> GetAllAsync();
        Task<List<Category>> GetAllActiveAsync();
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(long id);
        Task<List<Category>> GetChildCategoriesAsync(long parentId);
    }
}
