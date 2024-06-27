using AutoMapper;
using ChatApp.Application.Features.Messages.Command.AddMessage;
using ChatApp.Application.Features.Messages.Queries.GetMessageForUser;
using ChatApp.Application.Persistence.Contracts;
using ChatApp.Core.Entities;
using ChatApp.Domain.Entities;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ChatApp.API.SignalR
{
    public class MessageHub : Hub
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IHubContext<PresenceHub> _presenceHub;
        private readonly PresenceTracker _presenceTracker;

        public MessageHub(
            IMessageRepository messageRepository,
            IMapper mapper,
            IUserRepository userRepository,
            IHubContext<PresenceHub> presenceHub,
            PresenceTracker presenceTracker
            )
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _presenceHub = presenceHub;
            _presenceTracker = presenceTracker;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var currentUserName = Context.User?.FindFirstValue(ClaimTypes.GivenName);
            var otherUserName = httpContext?.Request.Query["user"].ToString();
            var groupName = GetGroupName(currentUserName, otherUserName);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var group = await AddToGroup(groupName);

            await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

            var messages = await _messageRepository.GetMessagesIsReadAsync(currentUserName, otherUserName);
            await Clients.Caller.SendAsync("ReceiveMessageRead", messages);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var group = await RemoveFromMessageGroup();
            await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
            await base.OnDisconnectedAsync(exception);
        }

        // Send Message
        public async Task SendMessage(AddMessageDto addMessageDto)
        {
            var senderUserName = Context.User?.FindFirstValue(ClaimTypes.GivenName);

            if (senderUserName == addMessageDto.RecipientUserName.ToLower())
                throw new HubException("You cann't send message to yourself");

            var sender = await _userRepository.GetUserByUserNameAsync(senderUserName);

            var recipient = await _userRepository.GetUserByUserNameAsync(addMessageDto.RecipientUserName);

            if (recipient is null)
                throw new HubException("Not Found User");

            var message = new Message()
            {
                Sender = sender,
                Recipient = recipient,
                SenderUserName = sender.UserName,
                RecipientUserName = recipient.UserName,
                Content = addMessageDto.Content
            };

            var groupName = GetGroupName(sender.UserName, recipient.UserName);
            var group = await _messageRepository.GetMessageGroupAsync(groupName);
            
            if(group.Connections.Any(x=>x.UserName == recipient.UserName))
            {
                message.DateRead = DateTime.Now;
            }
            else
            {
                var connections = await _presenceTracker.GetConnectionsForUser(recipient.UserName);
                if (connections is not null)
                {
                    await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived", new
                    {
                        userName = sender.UserName,
                        knownAs = sender.KnownAs,
                        content = addMessageDto.Content
                    });
                }
            }

            _messageRepository.AddMessage(message);

            if (await _messageRepository.SaveAllAsync())
            {
                await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
            }

        }
        private string GetGroupName(string caller, string other)
        {
            var stringComparer = string.CompareOrdinal(caller, other) < 0;

            return stringComparer ? $"{caller}-{other}" : $"{other}-{caller}";
        }

        private async Task<Group> AddToGroup(string groupName)
        {
            var group = await _messageRepository.GetMessageGroupAsync(groupName);
            var connection = new Connection(Context.ConnectionId, Context.User.FindFirstValue(ClaimTypes.GivenName));
            if (group is null)
            {
                group = new Group(groupName);
                _messageRepository.AddGroup(group);
            }
            group.Connections.Add(connection);

            if( await _messageRepository.SaveAllAsync())
                return group;

            throw new HubException("Failed to join group");
        }

        private async Task<Group> RemoveFromMessageGroup()
        {
            var group = await _messageRepository.GetGroupForConnectionAsync(Context.ConnectionId);
            var connection = group.Connections.FirstOrDefault(x=>x.ConnectionId == Context.ConnectionId);
            _messageRepository.RemoveConnection(connection);
            if(await _messageRepository.SaveAllAsync())
                return group;

            throw new HubException("Failed to remove from group");
        }



    }
}
