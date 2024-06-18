using ChatApp.Application.Features.Messages.Queries.GetMessageForUser;
using ChatApp.Application.Persistence.Contracts;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ChatApp.Application.Features.Messages.Command.GetMessagesIsRead
{
    public class GetMessagesIsReadCommand : IRequest<IEnumerable<MessageDto>>
    {

        public string CurrentUserName { get; set; } = null!;
        public string ReceiverUserName { get; set; }
        public GetMessagesIsReadCommand(string receiverUserName)
        {
            ReceiverUserName = receiverUserName;
        }

        class Handler : IRequestHandler<GetMessagesIsReadCommand, IEnumerable<MessageDto>>
        {
            private readonly IMessageRepository _messageRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IMessageRepository messageRepository, IHttpContextAccessor httpContextAccessor)
            {
                _messageRepository = messageRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<IEnumerable<MessageDto>> Handle(GetMessagesIsReadCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    request.CurrentUserName = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty;

                    var message = await _messageRepository.GetMessagesIsReadAsync(request.CurrentUserName, request.ReceiverUserName);

                    return message;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

    }
}
