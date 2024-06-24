using AutoMapper;
using ChatApp.Application.Features.Messages.Command.AddMessage;
using ChatApp.Application.Features.Messages.Queries.GetMessageForUser;
using ChatApp.Application.Persistence.Contracts;
using ChatApp.Core.Entities;
using ChatApp.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ChatApp.API.SignalR
{
    public class MessageHub : Hub
    {
        private readonly IMessageRepository _messageRepository;
        private readonly ILogger<MessageHub> _logger;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly PresenceTracker _presenceTracker;

        public MessageHub(
            IMessageRepository messageRepository,
            ILogger<MessageHub> logger,
            IMediator mediator,
            IMapper mapper,
            UserManager<AppUser> userManager,
            PresenceTracker presenceTracker
            )
        {
            _messageRepository = messageRepository;
            _logger = logger;
            _mediator = mediator;
            _mapper = mapper;
            _userManager = userManager;
            _presenceTracker = presenceTracker;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var currentUserName = Context.User?.FindFirstValue(ClaimTypes.GivenName);
            var otherUserName = httpContext?.Request.Query["user"].ToString();

            if (string.IsNullOrEmpty(currentUserName) || string.IsNullOrEmpty(otherUserName))
            {
                Context.Abort();
                return;
            }

            var groupName = GetGroupName(currentUserName, otherUserName);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            var messages = await _messageRepository.GetMessagesIsReadAsync(currentUserName, otherUserName);
            await Clients.Group(groupName).SendAsync("ReceiveMessageRead", messages);
        }

        private string GetGroupName(string caller, string other)
        {
            var stringComparer = string.CompareOrdinal(caller, other) < 0;

            return stringComparer ? $"{caller}-{other}" : $"{other}-{caller}";
        }

        // Send Message
        public async Task SendMessage(AddMessageDto addMessageDto)
        {
            var senderUserName = Context.User?.FindFirstValue(ClaimTypes.GivenName);
            if (string.IsNullOrEmpty(senderUserName))
            {
                throw new HubException("Sender not authenticated");
            }

            var sender = await _userManager.Users
                .Include(x => x.Photos)
                .FirstOrDefaultAsync(x => x.UserName == senderUserName);

            var recipient = await _userManager.Users
                .Include(x => x.Photos)
                .FirstOrDefaultAsync(x => x.UserName == addMessageDto.RecipientUserName);

            if (recipient == null || sender == null)
            {
                throw new HubException("Invalid recipient or sender");
            }

            var message = _mapper.Map<Message>(addMessageDto);
            message.SenderId = sender.Id;
            message.SenderUserName = senderUserName;
            message.RecipientId = recipient.Id;

            await _messageRepository.AddAsync(message);

            //var messageDto = _mapper.Map<MessageDto>(message);

            var groupName = GetGroupName(sender.UserName, recipient.UserName);
            await Clients.Group(groupName).SendAsync("NewMessage", message);
        }


        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

    }
}
