using ChatApp.Application.Persistence.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ChatApp.API.SignalR
{
    [Authorize]
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker _presenceTracker;
        private readonly IUserRepository _userRepository;

        public PresenceHub(PresenceTracker presenceTracker, IUserRepository userRepository)
        {
            _presenceTracker = presenceTracker;
            _userRepository = userRepository;
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

            if (!string.IsNullOrEmpty(userName))
            {
                try
                {
                    var user = await _userRepository.GetUserByUserNameAsync(userName);

                    if (user is not null)
                    {
                        user.LastActive = DateTime.Now;
                        await _userRepository.UpdateUserAsync(user);
                    }
                }
                catch (Exception ex)
                {
                    throw new HubException(userName, ex);
                }
            }

            var isOffline = await _presenceTracker.DisconnectedUser(userName, Context.ConnectionId);
            if (isOffline)
                await Clients.Others.SendAsync("UserIsOffline", userName);

            await base.OnDisconnectedAsync(exception);
        }
    }
}
