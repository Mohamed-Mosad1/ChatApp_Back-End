using System.Collections.Concurrent;

namespace ChatApp.API.SignalR
{
    public class PresenceTracker
    {
        private static readonly ConcurrentDictionary<string, List<string>> OnlineUsers = new ConcurrentDictionary<string, List<string>>();

        public Task<bool> ConnectedUser(string userName, string connectionId)
        {
            OnlineUsers.AddOrUpdate(userName, new List<string> { connectionId }, (key, existingList) =>
            {
                lock (existingList)
                {
                    if (!existingList.Contains(connectionId))
                    {
                        existingList.Add(connectionId);
                    }
                }
                return existingList;
            });

            return Task.FromResult(true);
        }


        public Task<bool> DisconnectedUser(string userName, string connectionId)
        {
            bool isOffline = false;
            if (OnlineUsers.TryGetValue(userName, out var connections))
            {
                lock (connections)
                {
                    if (!connections.Contains(connectionId))
                        return Task.FromResult(isOffline);

                    connections.Remove(connectionId);
                    if (connections.Count == 0)
                    {
                        OnlineUsers.TryRemove(userName, out _);
                        isOffline = true;
                    }
                }
            }

            return Task.FromResult(isOffline);
        }


        public Task<string[]> GetOnlineUsers()
        {
            var onlineUsers = OnlineUsers.Keys.OrderBy(k => k).ToArray();
            return Task.FromResult(onlineUsers);
        }

        public static Task<List<string>> GetConnectionsForUser(string userName)
        {
            List<string> connectionIds = new List<string>();

            lock (OnlineUsers)
            {
                connectionIds = OnlineUsers.GetValueOrDefault(userName);
            }

            return Task.FromResult(connectionIds);
        }

    }
}
