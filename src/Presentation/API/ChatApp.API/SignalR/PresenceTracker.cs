using System.Collections.Concurrent;

namespace ChatApp.API.SignalR
{
    public class PresenceTracker
    {
        private static readonly ConcurrentDictionary<string, List<string>> OnlineUsers = new ConcurrentDictionary<string, List<string>>();

        public Task ConnectedUser(string userName, string connectionId)
        {
            OnlineUsers.AddOrUpdate(userName, new List<string> { connectionId }, (key, existingList) =>
            {
                existingList.Add(connectionId);
                return existingList;
            });

            return Task.CompletedTask;
        }

        public Task DisconnectedUser(string userName, string connectionId)
        {
            if (OnlineUsers.TryGetValue(userName, out var connections))
            {
                connections.Remove(connectionId);
                if (connections.Count == 0)
                {
                    OnlineUsers.TryRemove(userName, out _);
                }
            }

            return Task.CompletedTask;
        }

        public Task<string[]> GetOnlineUsers()
        {
            var onlineUsers = OnlineUsers.Keys.OrderBy(k => k).ToArray();
            return Task.FromResult(onlineUsers);
        }
    }
}
