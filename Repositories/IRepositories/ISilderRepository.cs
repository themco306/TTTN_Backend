using backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Repositories.IRepositories
{
    public interface ISilderRepository
    {
        Task<Slider> GetByIdAsync(long id);
        Task<List<Slider>> GetAllAsync();
        Task AddAsync(Slider gallery);
        Task UpdateAsync(Slider gallery);
        Task DeleteAsync(Slider gallery);
    }
}
