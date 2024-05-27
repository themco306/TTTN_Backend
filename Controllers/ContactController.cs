
using backend.DTOs;
using backend.Exceptions;
using backend.Helper;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
        [ApiController]
        [Route("api/contacts")]

        public class ContactController : ControllerBase
        {
                private readonly ContactService _contactService;
                private readonly IHttpContextAccessor _httpContextAccessor;
                public ContactController(ContactService contactService, IHttpContextAccessor httpContextAccessor)
                {
                        _contactService = contactService;
                        _httpContextAccessor = httpContextAccessor;
                }
                [HttpGet]
                 [Authorize(Policy =$"{AppRole.SuperAdmin}{ClaimType.ContactClaim}{ClaimValue.Show}")] 
                public async Task<IActionResult> GetContacts()
                {
                        var contacts = await _contactService.GetAllContactsAsync();
                        return Ok(contacts);
                }
                [HttpGet("{id}")]
                 [Authorize(Policy =$"{AppRole.SuperAdmin}{ClaimType.ContactClaim}{ClaimValue.Show}")] 
                public async Task<IActionResult> GetContact(long id)
                {
                        var contact = await _contactService.GetContactByIdAsync(id);
                        return Ok(contact);
                }

                [HttpPost]
                public async Task<IActionResult> PostContact(ContactInputDTO contactInputDTO)
                {

                        await _contactService.CreateContactAsync(contactInputDTO);
                        return Ok(new { message = "Gửi yêu cầu thành công, Chúng tôi sẽ phản hồi sớm nhất." });
                }

                [HttpPut("{id}")]
                [Authorize(Policy = $"{AppRole.SuperAdmin}{ClaimType.ContactClaim}{ClaimValue.Edit}")]

                public async Task<IActionResult> PutContact(long id, ContactReplayDTO contactReplayDTO)
                {
                        string tokenWithBearer = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
                        await _contactService.UpdateContactAsync(id, contactReplayDTO, tokenWithBearer);
                        return Ok(new { message = "Trả lời thành công" });
                }

                [HttpDelete("{id}")]
                [Authorize(Policy = $"{AppRole.SuperAdmin}{ClaimType.ContactClaim}{ClaimValue.Delete}")]
                public async Task<IActionResult> DeleteContact(long id)
                {
                        await _contactService.DeleteContactAsync(id);
                        return Ok(new { message = "Xóa thành công liên hệ có ID: " + id });
                }
                [HttpDelete("delete-multiple")]
                [Authorize(Policy = $"{AppRole.SuperAdmin}{ClaimType.ContactClaim}{ClaimValue.Delete}")]
                public async Task<IActionResult> DeleteMultipleContacts(LongIDsModel model)
                {
                        if (model.ids == null || model.ids.Count == 0)
                        {
                                return BadRequest(new { error = "Danh sách các ID không được trống." });
                        }

                        await _contactService.DeleteContactsAsync(model.ids);
                        string concatenatedIds = string.Join(", ", model.ids);
                        return Ok(new { message = "Xóa thành công liên hệ có ID: " + concatenatedIds });


                }
                [HttpPost("sendReplay/{email}")]
                [Authorize(Policy = $"{AppRole.SuperAdmin}{ClaimType.ContactClaim}{ClaimValue.Edit}")]
                public async Task<IActionResult> SendReplayEmail(string email, ContactReplayDTO contactReplayDTO)
                {
                        await _contactService.SendReplayEmail(email, contactReplayDTO);
                        return Ok(new { message = "TC" });
                }



        }
}
