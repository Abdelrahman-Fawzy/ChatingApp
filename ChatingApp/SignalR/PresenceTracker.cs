using System.Collections.Concurrent;

namespace ChatingApp.BackEnd.SignalR
{
    public class PresenceTracker
    {
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> _onlineUsers = new();

        public Task UserConnected(string userID, string connectionID)
        {
            var connections = _onlineUsers.GetOrAdd(userID, _ => new ConcurrentDictionary<string, byte>());
            connections.TryAdd(connectionID, 0);

            return Task.CompletedTask;
        }

        public Task userDisconnected(string userID, string connectionID)
        {
            if(_onlineUsers.TryGetValue(userID, out var connections))
            {
                connections.TryRemove(connectionID, out _);
                if(connections.IsEmpty)
                {
                    _onlineUsers.TryRemove(userID, out _);
                }
            }

            return Task.CompletedTask;
        }

        public Task<string[]> GetOnlineUsers()
        {
            return Task.FromResult(_onlineUsers.Keys.OrderBy(k => k).ToArray());
        }

        public static Task<List<string>> GetConnectionsForUser(string userID)
        {
            if(_onlineUsers.TryGetValue(userID, out var connections))
            {
                return Task.FromResult(connections.Keys.ToList());
            }
            return Task.FromResult(new List<string>());
        }
    }
}
