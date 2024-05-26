
using backend.Models;

namespace backend.Repositories.IRepositories
{
    public interface IBrandRepository
    {
        Task<Brand> GetByIdAsync(long id);
        Task<List<Brand>> GetAllAsync();
        Task<List<Brand>> GetAllActiveAsync();
        Task AddAsync(Brand brand);
        Task UpdateAsync(Brand brand);
        Task DeleteAsync(long id);

    }
}
