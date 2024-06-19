using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ChatApp.API.SignalR
{
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker _presenceTracker;
        private readonly ILogger<PresenceHub> _logger;

        public PresenceHub(PresenceTracker presenceTracker, ILogger<PresenceHub> logger)
        {
            _presenceTracker = presenceTracker;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var userName = Context.User?.FindFirstValue(ClaimTypes.GivenName);

            if (string.IsNullOrEmpty(userName))
            {
                _logger.LogWarning("User connection attempt without a valid username.");
                Context.Abort();
                return;
            }

            try
            {
                await _presenceTracker.ConnectedUser(userName, Context.ConnectionId);
                await Clients.Others.SendAsync("UserIsOnline", userName);

                var currentUsers = await _presenceTracker.GetOnlineUsers();
                await Clients.All.SendAsync("GetOnlineUsers", currentUsers);

                _logger.LogInformation($"{userName} connected with connection ID {Context.ConnectionId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during OnConnectedAsync");
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userName = Context.User?.FindFirstValue(ClaimTypes.GivenName);

            if (string.IsNullOrEmpty(userName))
            {
                _logger.LogWarning("User disconnection attempt without a valid username.");
                return;
            }

            try
            {
                await _presenceTracker.DisconnectedUser(userName, Context.ConnectionId);
                await Clients.Others.SendAsync("UserIsOffline", userName);

                var currentUsers = await _presenceTracker.GetOnlineUsers();
                await Clients.All.SendAsync("GetOnlineUsers", currentUsers);

                _logger.LogInformation($"{userName} disconnected with connection ID {Context.ConnectionId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during OnDisconnectedAsync");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
