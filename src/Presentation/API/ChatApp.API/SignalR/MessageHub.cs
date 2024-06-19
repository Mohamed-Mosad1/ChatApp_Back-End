using AutoMapper;
using ChatApp.Application.Features.Messages.Command.AddMessage;
using ChatApp.Application.Persistence.Contracts;
using ChatApp.Core.Entities;
using ChatApp.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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

        public MessageHub(
            IMessageRepository messageRepository, 
            ILogger<MessageHub> logger,
            IMediator mediator,
            IMapper mapper,
            UserManager<AppUser> userManager
            )
        {
            _messageRepository = messageRepository;
            _logger = logger;
            _mediator = mediator;
            _mapper = mapper;
            _userManager = userManager;
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                var currentUserName = Context.User?.FindFirstValue(ClaimTypes.GivenName);
                if (string.IsNullOrEmpty(currentUserName))
                {
                    _logger.LogWarning("User connected without a valid username.");
                    Context.Abort();
                    return;
                }

                var httpContext = Context.GetHttpContext();
                var otherUser = httpContext?.Request.Query["user"].ToString();

                if (string.IsNullOrEmpty(otherUser))
                {
                    _logger.LogWarning("User connected without specifying the other user.");
                    Context.Abort();
                    return;
                }

                var groupName = GetGroupName(currentUserName, otherUser);

                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

                var messages = await _messageRepository.GetMessagesIsReadAsync(currentUserName, otherUser);
                await Clients.Group(groupName).SendAsync("ReceiveMessageRead", messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during OnConnectedAsync.");
                throw;
            }

        }

        private string GetGroupName(string caller, string other)
        {
            var stringComparer = string.CompareOrdinal(caller, other) < 0;

            return stringComparer ? $"{caller}-{other}" : $"{other}-{caller}";
        }

        // Send Message
        public async Task SendMessage(AddMessageDto addMessageDto)
        {   
            var message = _mapper.Map<Message>(addMessageDto);

            message.SenderId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            message.SenderUserName = Context.User.FindFirstValue(ClaimTypes.GivenName);

            var recipient = await _userManager.Users.Include(x => x.Photos).FirstOrDefaultAsync(x => x.UserName == message.RecipientUserName);
            var sender = await _userManager.Users.Include(x => x.Photos).FirstOrDefaultAsync(x => x.UserName == message.SenderUserName);
            
            message.RecipientId = recipient?.Id;
            await _messageRepository.AddAsync(message);
            var caller = sender?.UserName;
            var other = recipient?.UserName;

            var groupName = GetGroupName(caller, other);
            await Clients.Group(groupName).SendAsync("NewMessage", message);
            
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

    }
}
