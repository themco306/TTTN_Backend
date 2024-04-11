using backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Repositories.IRepositories
{
    public interface IGalleryRepository
    {
        Task<Gallery> GetByIdAsync(long id);
        Task<List<Gallery>> GetAllAsync();
        Task AddAsync(Gallery gallery);
        Task UpdateAsync(Gallery gallery);
        Task DeleteAsync(long id);
        Task<List<Gallery>> GetGalleriesByProductIdAsync(long productId);
    }
}
