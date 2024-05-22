using backend.Context;
using backend.Models;
using backend.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly AppDbContext _context;

        public PostRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Post> GetByIdAsync(long id)
        {
            return await _context.Posts
            .Include(c=>c.CreatedBy)
            .Include(c=>c.UpdatedBy)
            .FirstAsync(p => p.Id == id);
        }

        public async Task<List<Post>> GetAllAsync(bool active=false)
        {
            if(active==true){ 
                 return await _context.Posts
            .Where(c=>c.Status==1)
            .OrderByDescending(c=>c.UpdatedAt)
            .ToListAsync();
            }else{
                  return await _context.Posts
            .OrderByDescending(c=>c.UpdatedAt)
            .ToListAsync();
            }
          
        }

        public async Task AddAsync(Post post)
        {
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Post post)
        {
            _context.Entry(post).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Post post)
        {
            if (post != null)
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
            }
        }
    }
}
