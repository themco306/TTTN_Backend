namespace backend.Hubs
{
    public class UserTracker
    {
        private readonly Dictionary<string, string> _onlineUsers = new Dictionary<string, string>();

        public Task AddUser(string connectionId, string userId)
        {
            lock (_onlineUsers)
            {
                // Chỉ thêm userId vào Dictionary nếu nó chưa tồn tại
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
                // Xóa userId từ Dictionary nếu connectionId tương ứng tồn tại
                var userIdToRemove = _onlineUsers.FirstOrDefault(x => x.Value == connectionId).Key;
                if (userIdToRemove != null)
                {
                    _onlineUsers.Remove(userIdToRemove);
                }
            }
            return Task.CompletedTask;
        }

        public Task<int> GetOnlineUserCount()
        {
            lock (_onlineUsers)
            {
                return Task.FromResult(_onlineUsers.Count);
            }
        }

        public Task<List<string>> GetOnlineUsers()
        {
            lock (_onlineUsers)
            {
                return Task.FromResult(_onlineUsers.Keys.ToList());
            }
        }
    }
}
