using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ChatApp.API.SignalR
{
    [Authorize]
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker _presenceTracker;

        public PresenceHub(PresenceTracker presenceTracker)
        {
            _presenceTracker = presenceTracker;
        }

        public override async Task OnConnectedAsync()
        {
            var userName = Context.User?.FindFirstValue(ClaimTypes.GivenName);

            var isOnline = await _presenceTracker.ConnectedUser(userName, Context.ConnectionId);
            if (isOnline)
                await Clients.Others.SendAsync("UserIsOnline", userName);

            var currentUsers = await _presenceTracker.GetOnlineUsers();
            await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);

        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userName = Context.User?.FindFirstValue(ClaimTypes.GivenName);

            var isOffline = await _presenceTracker.DisconnectedUser(userName, Context.ConnectionId);
            if (isOffline)
                await Clients.Others.SendAsync("UserIsOffline", userName);

            await base.OnDisconnectedAsync(exception);
        }
    }
}
