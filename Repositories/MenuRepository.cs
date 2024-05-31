using backend.Context;
using backend.Models;
using backend.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Repositories
{
    public class MenuRepository : IMenuRepository
    {
        private readonly AppDbContext _context;

        public MenuRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Menu> GetByIdAsync(long id)
        {
            return await _context.Menus.
            Include(c=>c.Parent)
            .Include(c=>c.CreatedBy)
            .Include(c=>c.UpdatedBy)
            .FirstOrDefaultAsync(p => p.Id == id);
        }
                public async Task<Menu> GetByLinkAsync(string link)
        {
            return await _context.Menus.
            Where(c=>c.Status == 1).
            Include(c=>c.Parent)
            .Include(c=>c.CreatedBy)
            .Include(c=>c.UpdatedBy)
            .FirstOrDefaultAsync(p => p.Link == link);
        }
                public async Task<List<Menu>> GetMenuHeaderAsync()
        {
            return await _context.Menus
            .Where(c=>c.Position=="mainmenu"&&c.Status==1&&c.Parent==null)
            .OrderBy(c=>c.SortOrder)
            .ToListAsync();
        }
                        public async Task<List<Menu>> GetMenuFooterAsync()
        {
            return await _context.Menus
            .Where(c=>c.Position=="footermenu"&&c.Status==1)
            .OrderBy(c=>c.SortOrder)
            .ToListAsync();
        }
                        public async Task<List<Menu>> GetSubMenusAsync(long id)
        {
            return await _context.Menus
            .Where(c=>c.Status==1&&c.ParentId==id)
            .OrderBy(c=>c.SortOrder)
            .ToListAsync();
        }
        public async Task<Menu> GetByTableIdAsync(long id,string type)
        {
            return await _context.Menus
            .FirstOrDefaultAsync(p => p.TableId == id&&p.Type==type);
        }
public async Task<List<Menu>> GetAllAsync(bool active = false, bool noParent = false)
{
    IQueryable<Menu> query = _context.Menus;

    if (active)
    {
        query = query.Where(c => c.Status == 1);
    }

    if (noParent)
    {
        query = query.Where(c => c.ParentId == null);
    }

    return await query.OrderByDescending(c => c.UpdatedAt).ToListAsync();
}

        public async Task AddAsync(Menu menu)
        {
            _context.Menus.Add(menu);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Menu menu)
        {
            _context.Entry(menu).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Menu menu)
        {
            if (menu != null)
            {
                _context.Menus.Remove(menu);
                await _context.SaveChangesAsync();
            }
        }
    }
}
