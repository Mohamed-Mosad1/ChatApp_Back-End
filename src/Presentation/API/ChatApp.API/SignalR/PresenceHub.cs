using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ChatApp.API.SignalR
{
    public class PresenceHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userName = Context.User?.FindFirstValue(ClaimTypes.GivenName);

            await Clients.Others.SendAsync("UserIsOnline", userName);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userName = Context.User?.FindFirstValue(ClaimTypes.GivenName);

            await Clients.Others.SendAsync("UserIsOffline", userName);

            await base.OnDisconnectedAsync(exception);
        }
    }
}
