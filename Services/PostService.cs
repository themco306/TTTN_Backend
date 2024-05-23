using AutoMapper;
using backend.DTOs;
using backend.Exceptions;
using backend.Helper;
using backend.Models;
using backend.Repositories.IRepositories;


namespace backend.Services
{
    public class PostService
    {
        private readonly IPostRepository _postRepository;
        private readonly GalleryService _galleryService;
        private readonly Generate _generate;
        private readonly IMapper _mapper;
        private readonly AccountService _accountService;
        private readonly TopicService _topicService;

        public PostService(IPostRepository postRepository, GalleryService galleryService, Generate generate, IMapper mapper, AccountService accountService, TopicService topicService)
        {
            _postRepository = postRepository;
            _galleryService = galleryService;
            _generate = generate;
            _mapper = mapper;
            _accountService = accountService;
            _topicService = topicService;
        }

        public async Task<List<PostGetDTO>> GetPostsAsync()
        {
            var posts = await _postRepository.GetAllAsync();

            return _mapper.Map<List<PostGetDTO>>(posts);
        }
                public async Task<List<PostGetDTO>> GetPostsPageAsync()
        {
            var posts = await _postRepository.GetAllPageAsync();

            return _mapper.Map<List<PostGetDTO>>(posts);
        }
        public async Task<List<PostGetDTO>> GetPostsActiveAsync()
        {
            var posts = await _postRepository.GetAllAsync(true);
            return _mapper.Map<List<PostGetDTO>>(posts);
        }
        public async Task<PostGetShowDTO> GetPostByIdAsync(long postId)
        {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null)
            {
                throw new NotFoundException("Bài viết không tồn tại");
            }

            return _mapper.Map<PostGetShowDTO>(post);
        }
        public async Task<PostGetShowDTO> GetPostShowByIdAsync(long postId)
        {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null)
            {
                throw new NotFoundException("Hình ảnh không tồn tại");
            }

                        return _mapper.Map<PostGetShowDTO>(post);

        }

        public async Task<PostGetDTO> CreateTypePostAsync(PostInputDTO postDTO, string token)
        {
            var userId = _accountService.ExtractUserIdFromToken(token);
            var existingUser = await _accountService.GetUserByIdAsync(userId);

            var slugName = _generate.GenerateSlug(postDTO.Name);
            var post = new Post
            {
                Name = postDTO.Name,
                Detail = postDTO.Detail,
                Slug=slugName,
                Type = PostType.post,
                ImagePath = await _galleryService.UploadImage(slugName, postDTO.Image, "posts"),
                Status = postDTO.Status,
                CreatedById = existingUser.Id,
                UpdatedById = existingUser.Id
            };
            if (postDTO.TopicId != 0)
            {
                var topic = await _topicService.GetTopicByIdAsync(postDTO.TopicId);
                if (topic == null)
                {
                    throw new NotFoundException("Chủ đề không tồn tại");
                }
                post.TopicId = topic.Id;
            }

            await _postRepository.AddAsync(post);
            return _mapper.Map<PostGetDTO>(post);
        }
        public async Task<PostGetDTO> CreateTypePageAsync(PostInputDTO postDTO, string token)
        {
            var userId = _accountService.ExtractUserIdFromToken(token);
            var existingUser = await _accountService.GetUserByIdAsync(userId);
            var slugName = _generate.GenerateSlug(postDTO.Name);
            var post = new Post
            {
                Name = postDTO.Name,
                Slug=slugName,
                Detail = postDTO.Detail,
                Type = PostType.page,
                TopicId=null,
                ImagePath = null,
                Status = postDTO.Status,
                CreatedById = existingUser.Id,
                UpdatedById = existingUser.Id
            };
            await _postRepository.AddAsync(post);
            return _mapper.Map<PostGetDTO>(post);
        }
        public async Task<PostGetShowDTO> UpdatePostAsync(long postId, PostInputDTO postDTO, string token)
        {
            var userId = _accountService.ExtractUserIdFromToken(token);
            var existingUser = await _accountService.GetUserByIdAsync(userId);
            var existingPost = await _postRepository.GetByIdAsync(postId);
            if (existingPost == null)
            {
                throw new NotFoundException("Không tồn tại");
            }
             var slugName = _generate.GenerateSlug(postDTO.Name);
            if(existingPost.Type==PostType.post){
                var topic = await _topicService.GetTopicByIdAsync(postDTO.TopicId);
                if (topic == null)
                {
                    throw new NotFoundException("Chủ đề không tồn tại");
                }
                existingPost.TopicId=postDTO.TopicId;
                 if (postDTO.Image != null)
            {
               
                await _galleryService.DeleteImageAsync(existingPost.ImagePath, "posts");
                existingPost.ImagePath = await _galleryService.UploadImage(slugName, postDTO.Image, "posts");
            }
            }
            
            existingPost.Name = postDTO.Name;
            existingPost.UpdatedById = existingUser.Id;
            existingPost.Slug=slugName;
            existingPost.Detail = postDTO.Detail;
           
            await _postRepository.UpdateAsync(existingPost);

            return _mapper.Map<PostGetShowDTO>(existingPost);

        }

        public async Task DeletePostAsync(long postId)
        {
            var existingPost = await _postRepository.GetByIdAsync(postId);
            if (existingPost == null)
            {
                throw new NotFoundException("Không tồn tại");
            }
            await _galleryService.DeleteImageAsync(existingPost.ImagePath, "posts");
            await _postRepository.DeleteAsync(existingPost);
        }
        public async Task DeletePostsAsync(List<long> ids)
        {
            foreach (var id in ids)
            {
                var existingCategory = await _postRepository.GetByIdAsync(id);
                if (existingCategory != null)
                {
                    await _galleryService.DeleteImageAsync(existingCategory.ImagePath, "posts");
                    await _postRepository.DeleteAsync(existingCategory);
                }
            }
        }
        public async Task UpdatePostStatusAsync(long id)
        {
            var existing = await _postRepository.GetByIdAsync(id);

            if (existing == null)
            {
                throw new NotFoundException("Không tồn tại");
            }
            existing.Status = existing.Status == 0 ? 1 : 0;

            await _postRepository.UpdateAsync(existing);
        }
    }
}
