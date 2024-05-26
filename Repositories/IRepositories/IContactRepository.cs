
using backend.Models;

namespace backend.Repositories.IRepositories
{
    public interface IContactRepository
    {
        Task<Contact> GetByIdAsync(long id);
        Task<List<Contact>> GetAllAsync();
        Task<List<Contact>> GetAllActiveAsync();
        Task AddAsync(Contact contact);
        Task UpdateAsync(Contact contact);
        Task DeleteAsync(long id);

    }
}
