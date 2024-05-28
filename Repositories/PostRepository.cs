using backend.Context;
using backend.DTOs;
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
            .Include(c=>c.Topic)
            .Include(c=>c.CreatedBy)
            .Include(c=>c.UpdatedBy)
            .FirstAsync(p => p.Id == id);
        }
        public async Task<Post> GetBySlugAsync(string slug)
        {
            return await _context.Posts
            .Include(c=>c.Topic)
            .Include(c=>c.CreatedBy)
            .Include(c=>c.UpdatedBy)
            .FirstAsync(p => p.Slug == slug&&p.Status==1);
        }
        public async Task<List<Post>> GetPostSameAsync(long id,long topicId)
        {
                 return await _context.Posts
            .Where(c=>c.Status==1&&c.Type==PostType.post&&c.TopicId==topicId&&c.Id!=id)
            .Include(c=>c.Topic)
            .Take(8)
            .OrderByDescending(c=>c.UpdatedAt)
            .ToListAsync();
        }
        public async Task<List<Post>> GetAllAsync(bool active=false)
        {
            if(active==true){ 
                 return await _context.Posts
            .Where(c=>c.Status==1&&c.Type==PostType.post)
            .Include(c=>c.Topic)
            .OrderByDescending(c=>c.UpdatedAt)
            .ToListAsync();
            }else{
                  return await _context.Posts
            .Where(c=>c.Type==PostType.post)
            .Include(c=>c.Topic)
            .OrderByDescending(c=>c.UpdatedAt)
            .ToListAsync();
            }
          
        }
        public async Task<PagedResult<Post>> GetFilteredPostsAsync(PostFilterDTO filter)
{
    var query = _context.Posts
    .Where(c=>c.Status==1&&c.Type==PostType.post)
        .Include(c => c.Topic)
        .AsQueryable();

if (filter.TopicId!=null)
{
    query = query.Where(p => p.Topic.Slug == filter.TopicId);
}


    if (!string.IsNullOrEmpty(filter.SortBy))
    {
        switch (filter.SortBy.ToLower())
        {
            case "name":
                query = filter.SortOrder == "asc" ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name);
                break;
            case "date":
                query = filter.SortOrder == "asc" ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt);
                break;
            default:
                query = query.OrderByDescending(p => p.UpdatedAt);
                break;
        }
    }
    else
    {
        query = query.OrderByDescending(p => p.UpdatedAt);
    }

    var totalItems = await query.CountAsync();

    var items = await query
        .Skip((filter.PageNumber - 1) * filter.PageSize)
        .Take(filter.PageSize)
        .ToListAsync();

    return new PagedResult<Post>
    {
        Items = items,
        TotalCount = totalItems,
        PageSize = filter.PageSize,
        CurrentPage = filter.PageNumber
    };
}
        public async Task<List<Post>> GetAllPageAsync(bool active=false)
        {
            if(active==true){ 
                 return await _context.Posts
            .Where(c=>c.Status==1&&c.Type==PostType.page)
            .OrderByDescending(c=>c.UpdatedAt)
            .ToListAsync();
            }else{
                  return await _context.Posts
            .Where(c=>c.Type==PostType.page)

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
