
using backend.Models;

namespace backend.Repositories.IRepositories
{
    public interface IProductTagRepository
    {
        Task AddAsync(ProductTag productTag);
        Task DeleteAsync(long productId,long tagId);
    }
}
