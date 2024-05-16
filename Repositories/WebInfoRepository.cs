
using backend.Context;
using backend.Models;
using backend.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class WebInfoRepository : IWebInfoRepository
    {
        private readonly AppDbContext _context;

        public WebInfoRepository(AppDbContext context)
        {
            _context = context;
        }


        public async Task<WebInfo> GetByIdAsync(long id)
        {
            return await _context.WebInfos
                .FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<WebInfo> GetFirstAsync()
        {
            return await _context.WebInfos
            .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(WebInfo webInfo)
        {
            _context.Entry(webInfo).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

    }
}
