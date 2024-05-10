
using AutoMapper;
using backend.DTOs;
using backend.Exceptions;
using backend.Helper;
using backend.Models;
using backend.Repositories.IRepositories;


namespace backend.Services
{
    public class TagService
    {
        private readonly ITagRepository _tagRepository;

        public TagService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
    
        }
        public async Task<List<Tag>> GetAllTagAsync()
        {
            var tag = await _tagRepository.GetAllAsync();
            return tag;
        }
      
        public async Task<Tag> GetTagByTypeAsync(TagType type)
        {
            var tag = await _tagRepository.GetByTypeAsync(type);
            if (tag == null)
            {
                throw new NotFoundException("Thẻ không tồn tại.");
            }
            return tag;
        }
        public async Task<Tag> UpdateTagNameAsync(long id, string name)
        {
            var tag= await _tagRepository.GetByIdAsync(id);
            if (tag == null)
            {
                 throw new NotFoundException("Thẻ không tồn tại.");
            }
            tag.Name = name;
            await _tagRepository.UpdateAsync(tag);
            return tag;
        }
    public async Task UpdateTagOrderAsync(List<long> orderedIds)
        {
            var tags = await _tagRepository.GetAllAsync();
            var tagDictionary = tags.ToDictionary(t => t.Id);
            
            int order = 1;
            foreach (var id in orderedIds)
            {
                if (tagDictionary.ContainsKey(id))
                {
                    var tag = tagDictionary[id];
                    tag.Sort = order;
                    order++;
                }
            }

            // Lưu các thay đổi vào cơ sở dữ liệu
            foreach (var tag in tags)
            {
                await _tagRepository.UpdateAsync(tag);
            }
        }


    }
}
