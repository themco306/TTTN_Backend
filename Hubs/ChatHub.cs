using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Security.Claims;

namespace backend.Hubs
{
    [Authorize] // Yêu cầu xác thực để sử dụng ChatHub
    public class ChatHub : Hub
    {
        private readonly UserTracker _userTracker;

        public ChatHub(UserTracker userTracker)
        {
            _userTracker = userTracker;
        }

public override async Task OnConnectedAsync()
{
    var userId = Context.UserIdentifier;
    if (!string.IsNullOrEmpty(userId))
    {
        await _userTracker.AddUser(Context.ConnectionId, userId);
        await Clients.All.SendAsync("UserCountUpdated", await _userTracker.GetOnlineUserCount());
    }
    await base.OnConnectedAsync();
}

public override async Task OnDisconnectedAsync(Exception exception)
{
    await _userTracker.RemoveUser(Context.ConnectionId);
    await Clients.All.SendAsync("UserCountUpdated", await _userTracker.GetOnlineUserCount());
    await base.OnDisconnectedAsync(exception);
}


        public Task<int> GetOnlineUserCount()
        {
            return _userTracker.GetOnlineUserCount();
        }
    }
}
