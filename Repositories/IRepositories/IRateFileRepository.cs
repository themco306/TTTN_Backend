using backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Repositories.IRepositories
{
    public interface IRateFileRepository
    {
        Task<RateFile> GetByIdAsync(long id);
        Task<List<RateFile>> GetAllAsync();
        Task AddAsync(RateFile rateFile);
        Task UpdateAsync(RateFile rateFile);
        Task DeleteAsync(long id);
        Task<List<RateFile>> GetRateFilesByRateIdAsync(long rateId);
    }
}
