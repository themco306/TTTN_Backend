using System.Security.Claims;
using backend.DTOs;
using backend.Helper;
using backend.Hubs;
using backend.Models;
using backend.Repositories.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

[ApiController]
[Route("api/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly UserTracker _userTracker;
    private readonly IMessageRepository _messageRepository;
    private readonly IMessageReadStatusRepository _messageReadStatusRepository;
    private readonly IServiceProvider _serviceProvider;
    private readonly IAccountRepository _accountRepository;

    public NotificationsController(
        IHubContext<ChatHub> hubContext,
        UserTracker userTracker,
        IMessageRepository messageRepository,
        IServiceProvider serviceProvider,
        IAccountRepository accountRepository,
        IMessageReadStatusRepository messageReadStatusRepository)
    {
        _hubContext = hubContext;
        _userTracker = userTracker;
        _messageRepository = messageRepository;
        _serviceProvider = serviceProvider;
        _accountRepository = accountRepository;
        _messageReadStatusRepository=messageReadStatusRepository;
    }

    [HttpPost("send-message")]
    [Authorize]
    public async Task<IActionResult> SendMessageToAdmins([FromBody] NotificationRequest request)
    {
        var now = DateTime.UtcNow;
        now = TimeZoneInfo.ConvertTimeFromUtc(now, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
        List<string> adminConnectionIds = new List<string>();

        var message = new Message
        {
            Content = request.Message,
            CreatedAt = now,
            UpdatedAt=now,
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
        };
        await _messageRepository.AddAsync(message);

        var senderInfo = await GetShortUserInfo(message.UserId);

        var messageDTO = new MessageDTO
        {
            Id = message.Id,
            Content = message.Content,
            CreatedAt = message.CreatedAt,
            UpdatedAt=message.UpdatedAt,
            UserId = message.UserId,
            User = senderInfo
        };

        foreach (var userId in _userTracker.GetOnlineUsers().Result)
        {
            var roles = await _accountRepository.GetUserRolesAsync(userId);
            if (roles.Contains(AppRole.SuperAdmin) || roles.Contains(AppRole.Admin))
            {
                if (_userTracker.TryGetConnectionId(userId, out var connectionId))
                {
                    adminConnectionIds.Add(connectionId);
                }
            }
        }

        foreach (var connectionId in adminConnectionIds)
        {
            await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessage", messageDTO);
        }

        return Ok(new { message = "Gửi thành công" });
    }

    [HttpPost("mark-as-read")]
    [Authorize]
    public async Task<IActionResult> MarkMessageAsRead([FromBody] MarkMessageAsReadRequest request)
    {
         var now = DateTime.UtcNow;
        now = TimeZoneInfo.ConvertTimeFromUtc(now, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var message=await _messageRepository.GetByIdAsync(request.MessageId);
        if(message==null){
            return BadRequest("Thông báo không tồn tại");
        }
        await _messageReadStatusRepository.MarkMessageAsReadAsync(request.MessageId, userId);
        message.UpdatedAt=now;
        await _messageRepository.UpdateAsync(message);

        return Ok(new { message = "Đánh dấu tin nhắn là đã đọc thành công" });
    }
    [HttpPost("is-read")]
    [Authorize]
    public async Task<IActionResult> IsMessageRead([FromBody] MarkMessageAsReadRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var isRead = await _messageReadStatusRepository.IsMessageReadByUserAsync(request.MessageId, userId);

        return Ok(new { isRead });
    }
    [HttpDelete("messages/{id}")]
public async Task<IActionResult> DeleteMessage(int id)
{
    var message = await _messageRepository.GetByIdAsync(id);
    if (message == null)
    {
        return NotFound();
    }
    await _messageRepository.DeleteAsync(id);
     await _hubContext.Clients.All.SendAsync("MessageDeleted", id);
    return NoContent();
}

    private async Task<UserGetShortDTO> GetShortUserInfo(string userId)
    {
        var user = await _accountRepository.GetUserByIdAsync(userId);

        var userDTO = new UserGetShortDTO
        {
            Id = user.Id,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Avatar = user.Avatar,
        };

        return userDTO;
    }
}

public class NotificationRequest
{
    public string Message { get; set; }
}

public class MarkMessageAsReadRequest
{
    public long MessageId { get; set; }
}
