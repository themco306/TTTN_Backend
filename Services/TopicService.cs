using AutoMapper;
using backend.DTOs;
using backend.Exceptions;
using backend.Helper;
using backend.Models;
using backend.Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Services
{
    public class TopicService
    {
        private readonly ITopicRepository _topicRepository;
        private readonly IMapper _mapper;
        private readonly Generate _generate;
        private readonly AccountService _accountService;

        public TopicService(ITopicRepository topicRepository, IMapper mapper, Generate generate, AccountService accountService)
        {
            _topicRepository = topicRepository;
            _mapper = mapper;
            _generate = generate;
            _accountService = accountService;
        }

        public async Task<List<Topic>> GetAllTopicsAsync()
        {
            var topics = await _topicRepository.GetAllAsync();
            return topics;
        }
        public async Task<Topic> GetTopicShowAllByIdAsync(long id)
        {
            var topic = await _topicRepository.GetShowAllByIdAsync(id);
            if (topic == null)
            {
                throw new NotFoundException("Chủ đề không tồn tại.");
            }
            return topic;
        }
        public async Task<Topic> GetTopicByIdAsync(long id)
        {
            var topic = await _topicRepository.GetByIdAsync(id);
            if (topic == null)
            {
                throw new NotFoundException("Chủ đề không tồn tại.");
            }
            return topic;
        }
    
        public async Task<Topic> CreateTopicAsync(TopicInputDTO topicInputDTO, string token)
        {
            var userId = _accountService.ExtractUserIdFromToken(token);
            var existingUser = await _accountService.GetUserByIdAsync(userId);
             var topic=new Topic{
            Name=topicInputDTO.Name,
            Slug=_generate.GenerateSlug(topicInputDTO.Name),
            Status=topicInputDTO.Status,
            CreatedById=existingUser.Id,
            UpdatedById=existingUser.Id
           };
            if(topicInputDTO.ParentId!=0){
            var existingParent= await _topicRepository.GetByIdAsync(topicInputDTO.ParentId);
            if(existingParent == null){
                throw new NotFoundException("Chủ đề cha không tồn tại");
            }
            topic.ParentId=topicInputDTO.ParentId;
            }
            await _topicRepository.AddAsync(topic);
            return topic;
          
        }
        public async Task<Topic> UpdateTopicAsync(long id, TopicUpdateDTO topicInputDTO,string token)
        {
            var userId = _accountService.ExtractUserIdFromToken(token);
            var existingUser = await _accountService.GetUserByIdAsync(userId);
            var existingTopic = await _topicRepository.GetByIdAsync(id);

            if (existingTopic == null)
            {
                throw new NotFoundException("Chủ đề không tồn tại.");
            }
             if(topicInputDTO.ParentId!=0){
            var existingParent= await _topicRepository.GetByIdAsync(topicInputDTO.ParentId);
            if(existingParent == null){
                throw new NotFoundException("Chủ đề cha không tồn tại");
            }
            existingTopic.ParentId=topicInputDTO.ParentId;
            }
            existingTopic.Name=topicInputDTO.Name;
            existingTopic.Slug=_generate.GenerateSlug(topicInputDTO.Name);
            existingTopic.Status=topicInputDTO.Status;
            existingTopic.SortOrder=topicInputDTO.SortOrder;
            existingTopic.UpdatedById=existingUser.Id;
            // Update topic in the database
            await _topicRepository.UpdateAsync(existingTopic);

            return existingTopic;
        }
        public async Task DeleteTopicsById(List<long> ids)
        {
            foreach (var id in ids)
            {
                await DeleteTopicAsync(id);
            }
        }
        public async Task DeleteTopicAsync(long id)
        {
            var existingTopic = await _topicRepository.GetByIdAsync(id);

            if (existingTopic == null)
            {
                throw new NotFoundException("Mã giảm giá không tồn tại.");
            }

            await _topicRepository.DeleteAsync(id);
        }
         public async Task UpdateStatusAsync(long id)
        {
            var existing = await _topicRepository.GetByIdAsync(id);

            if (existing == null)
            {
                throw new NotFoundException("Mã giảm giá không tồn tại");
            }

            // Cập nhật trạng thái mới (nếu hiện tại là 0 thì cập nhật thành 1, và ngược lại)
            existing.Status = existing.Status == 0 ? 1 : 0;

            await _topicRepository.UpdateAsync(existing);
        }
    }
}
