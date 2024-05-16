
using backend.Models;

namespace backend.Repositories.IRepositories
{
    public interface ITagRepository
    {
        Task<Tag> GetByTypeAsync(TagType tagType);
        Task<Tag> GetByIdAsync(long id);
        Task<List<Tag>> GetAllAsync(bool active=false);
        Task UpdateAsync(Tag Tag);
    }
}
