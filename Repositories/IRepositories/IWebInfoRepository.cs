
using backend.Models;

namespace backend.Repositories.IRepositories
{
    public interface IWebInfoRepository
    {
        Task<WebInfo> GetByIdAsync(long id);
        Task<WebInfo> GetFirstAsync();
        Task UpdateAsync(WebInfo webInfo);
    }
}
