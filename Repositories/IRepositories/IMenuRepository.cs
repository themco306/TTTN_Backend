using backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Repositories.IRepositories
{
    public interface IMenuRepository
    {
        Task<Menu> GetByIdAsync(long id);
        Task<Menu> GetByTableIdAsync(long id,string type);
        Task<List<Menu>> GetAllAsync(bool active=false);
        Task AddAsync(Menu menu);
        Task UpdateAsync(Menu menu);
        Task DeleteAsync(Menu menu);
    }
}
