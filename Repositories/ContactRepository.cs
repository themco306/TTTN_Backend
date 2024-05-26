
using backend.Context;
using backend.Models;
using backend.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class ContactRepository : IContactRepository
    {
        private readonly AppDbContext _context;

        public ContactRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Contact> GetByIdAsync(long id)
        {
            return await _context.Contacts
                .Include(c => c.UpdatedBy)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Contact>> GetAllAsync()
        {
            return await _context.Contacts
            .OrderByDescending(c=>c.UpdatedAt)
            .ToListAsync();
        }
        public async Task<List<Contact>> GetAllActiveAsync()
        {
            return await _context.Contacts.Where(c=>c.Status==1)
            .OrderByDescending(c=>c.UpdatedAt)
            .ToListAsync();
        }
        public async Task AddAsync(Contact contact)
        {
            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Contact contact)
        {
            _context.Entry(contact).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact != null)
            {
                _context.Contacts.Remove(contact);
                await _context.SaveChangesAsync();
            }
        }
    }
}
