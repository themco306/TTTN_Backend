
using AutoMapper;
using backend.DTOs;
using backend.Exceptions;
using backend.Helper;
using backend.Models;
using backend.Repositories.IRepositories;


namespace backend.Services
{
    public class ContactService
    {
        private readonly IContactRepository _contactRepository;
        private readonly IMapper _mapper;
        private readonly Generate _generate;
        private readonly AccountService _accountService;
        private readonly EmailService _emailService;

        public ContactService(IContactRepository contactRepository, IMapper mapper, Generate generate, AccountService accountService,EmailService emailService)
        {
            _contactRepository = contactRepository;
            _mapper = mapper;
            _generate = generate;
            _accountService = accountService;
            _emailService=emailService;
        }
        public async Task<List<ContactGetDTO>> GetAllContactsAsync()
        {
            var contacts = await _contactRepository.GetAllAsync();
            return _mapper.Map<List<ContactGetDTO>>(contacts);
        }
       public async Task<List<ContactGetDTO>> GetAllContactsActiveAsync()
        {
            var contacts = await _contactRepository.GetAllActiveAsync();
            return _mapper.Map<List<ContactGetDTO>>(contacts);
        }
        public async Task<ContactGetDTO> GetContactByIdAsync(long id)
        {
            var contact = await _contactRepository.GetByIdAsync(id);
            if (contact == null)
            {
                throw new NotFoundException("Danh mục không tồn tại.");
            }
            return _mapper.Map<ContactGetDTO>(contact);
        }
        public async Task CreateContactAsync(ContactInputDTO contactInputDTO)
        {
            var contact = new Contact{
                Name = contactInputDTO.Name,
                Email=contactInputDTO.Email,
                Phone=contactInputDTO.Phone,
                Content=contactInputDTO.Content,
                Status=0,
            };
            await _contactRepository.AddAsync(contact);
        }
        public async Task UpdateContactAsync(long id, ContactReplayDTO contactReplayDTO,string token)
        {
             var userId = _accountService.ExtractUserIdFromToken(token);
            if (userId == null)
            {
                throw new NotFoundException("Có lỗi xãy ra vui lòng đăng nhập lại");
            }
            var user = await _accountService.GetUserByIdAsync(userId);
            var existingContact = await _contactRepository.GetByIdAsync(id);

            if (existingContact == null)
            {
                throw new NotFoundException("Liên hệ không tồn tại");
            }
            existingContact.ReplayContent=contactReplayDTO.ReplayContent;
            existingContact.Status=1;
            existingContact.UpdatedById=user.Id;
            await _contactRepository.UpdateAsync(existingContact);


        }
         public async Task SendReplayEmail(string email, ContactReplayDTO contactReplayDTO )
        {
            await _emailService.SendEmailAsync(email, "Trả lời Liên hệ", contactReplayDTO.ReplayContent);
        }
        public async Task DeleteContactAsync(long id)
        {
            var existingContact = await _contactRepository.GetByIdAsync(id);
            if (existingContact == null)
            {
                throw new NotFoundException("Thương hiệu không tồn tại.");
            }


            await _contactRepository.DeleteAsync(id);
        }
        public async Task DeleteContactsAsync(List<long> ids)
        {
            foreach (var id in ids)
            {
                var existingContact = await _contactRepository.GetByIdAsync(id);
                if (existingContact != null)
                {
                    await _contactRepository.DeleteAsync(id);
                }
            }
        }


    }
}
