using ChatApp.Application.Helpers;
using ChatApp.Application.Persistence.Contracts;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ChatApp.Application.Features.Messages.Queries.GetMessageForUser
{
    public class GetMessageForUserQuery : IRequest<PagedList<MessageDto>>
    {
        public MessageParams MessageParams { get; }

        public GetMessageForUserQuery(MessageParams messageParams)
        {
            MessageParams = messageParams;
        }

        class Handler : IRequestHandler<GetMessageForUserQuery, PagedList<MessageDto>>
        {
            private readonly IMessageRepository _messageRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IMessageRepository messageRepository, IHttpContextAccessor httpContextAccessor)
            {
                _messageRepository = messageRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<PagedList<MessageDto>> Handle(GetMessageForUserQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    request.MessageParams.UserName = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.GivenName);
                    var message = await _messageRepository.GetMessagesForUserAsync(request.MessageParams);
                    return message;
                }
                catch (Exception ex)
                {
                    // Optionally log the exception
                    throw new Exception($"Error getting messages for user: {ex.Message}", ex);
                }
            }
        }
    }
}
