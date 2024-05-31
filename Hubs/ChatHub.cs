using backend.DTOs;
using backend.Helper;
using backend.Models;
using backend.Repositories.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace backend.Hubs
{
    [Authorize] // Yêu cầu xác thực để sử dụng ChatHub
    public class ChatHub : Hub
    {
        private readonly UserTracker _userTracker;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMessageRepository _messageRepository;
        private readonly IMessageReadStatusRepository _messageReadStatusRepository;

        public ChatHub(
            UserTracker userTracker,
            IServiceProvider serviceProvider,
            IMessageRepository messageRepository,
            IMessageReadStatusRepository messageReadStatusRepository)
        {
            _userTracker = userTracker;
            _serviceProvider = serviceProvider;
            _messageRepository = messageRepository;
            _messageReadStatusRepository = messageReadStatusRepository;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                await _userTracker.AddUser(Context.ConnectionId, userId);
                await Clients.All.SendAsync("UserCountUpdated", await _userTracker.GetOnlineUserCount());

                // Gửi tất cả tin nhắn hiện có đến người dùng mới kết nối
                var messages = await _messageRepository.GetAllAsync();
                foreach (var message in messages)
                {
                    var messageDTO = new MessageDTO
                    {
                        Id = message.Id,
                        Content = message.Content,
                        CreatedAt = message.CreatedAt,
                        UserId = message.UserId,
                        User = await GetShortUserInfo(message.UserId)
                    };
                    await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", messageDTO);
                }
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await _userTracker.RemoveUser(Context.ConnectionId);
            await Clients.All.SendAsync("UserCountUpdated", await _userTracker.GetOnlineUserCount());
            await base.OnDisconnectedAsync(exception);
        }

        public Task<UserOnlineDTO> GetOnlineUserCount()
        {
            return _userTracker.GetOnlineUserCount();
        }

        public async Task SendMessageToAdmins(MessageDTO messageDTO)
        {
            List<string> adminConnectionIds = new List<string>();

            using (var scope = _serviceProvider.CreateScope())
            {
                var accountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();

                foreach (var userId in _userTracker.GetOnlineUsers().Result)
                {
                    var roles = await accountRepository.GetUserRolesAsync(userId);
                    if (roles.Contains(AppRole.SuperAdmin) || roles.Contains(AppRole.Admin))
                    {
                        if (_userTracker.TryGetConnectionId(userId, out var connectionId))
                        {
                            adminConnectionIds.Add(connectionId);
                        }
                    }
                }
            }

            foreach (var connectionId in adminConnectionIds)
            {
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", messageDTO);
            }
        }

        public async Task MarkMessageAsRead(long messageId)
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                var messageReadStatus = new MessageReadStatus
                {
                    MessageId = messageId,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
                await _messageReadStatusRepository.AddAsync(messageReadStatus);
            }
        }

        private async Task<UserGetShortDTO> GetShortUserInfo(string userId)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var accountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();
                var user = await accountRepository.GetUserByIdAsync(userId);

                return new UserGetShortDTO
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Avatar = user.Avatar,
                    // Thêm các trường thông tin khác cần thiết
                };
            }
        }
    }
}
