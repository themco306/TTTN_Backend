using backend.Helper;
using backend.Hubs;
using backend.Repositories.IRepositories;

public class UserTracker
{
    private readonly Dictionary<string, string> _onlineUsers = new Dictionary<string, string>();
    private readonly IServiceProvider _serviceProvider;

    public UserTracker(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task AddUser(string connectionId, string userId)
    {
        lock (_onlineUsers)
        {
            if (!_onlineUsers.ContainsKey(userId))
            {
                _onlineUsers[userId] = connectionId;
            }
        }
        return Task.CompletedTask;
    }

    public Task RemoveUser(string connectionId)
    {
        lock (_onlineUsers)
        {
            var userIdToRemove = _onlineUsers.FirstOrDefault(x => x.Value == connectionId).Key;
            if (userIdToRemove != null)
            {
                _onlineUsers.Remove(userIdToRemove);
            }
        }
        return Task.CompletedTask;
    }

    public async Task<UserOnlineDTO> GetOnlineUserCount()
    {
        int adminOnline = 0;
        int customerOnline = 0;

        using (var scope = _serviceProvider.CreateScope())
        {
            var accountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();

            foreach (var userId in _onlineUsers.Keys)
            {
                var roles = await accountRepository.GetUserRolesAsync(userId);
                if (roles.Contains(AppRole.SuperAdmin) || roles.Contains(AppRole.Admin))
                {
                    adminOnline++;
                }
                if (roles.Contains(AppRole.Customer))
                {
                    customerOnline++;
                }
            }
        }

        return new UserOnlineDTO
        {
            AdminOnline = adminOnline,
            CustomerOnline = customerOnline
        };
    }

    public Task<List<string>> GetOnlineUsers()
    {
        lock (_onlineUsers)
        {
            return Task.FromResult(_onlineUsers.Keys.ToList());
        }
    }

    public bool TryGetConnectionId(string userId, out string connectionId)
    {
        lock (_onlineUsers)
        {
            return _onlineUsers.TryGetValue(userId, out connectionId);
        }
    }
}
