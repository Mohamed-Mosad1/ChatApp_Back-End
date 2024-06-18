using ChatApp.Application.Persistence.Contracts;
using ChatApp.Application.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ChatApp.Application.Features.Messages.Command.DeleteMessage
{
    public class DeleteMessageCommand : IRequest<BaseCommonResponse>
    {
        public int Id { get; set; }

        public DeleteMessageCommand(int id)
        {
            Id = id;
        }

        class Handler : IRequestHandler<DeleteMessageCommand, BaseCommonResponse>
        {
            private readonly IMessageRepository _messageRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IMessageRepository messageRepository, IHttpContextAccessor httpContextAccessor)
            {
                _messageRepository = messageRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<BaseCommonResponse> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
            {
                var res = new BaseCommonResponse();
                try
                {
                    var userName = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.GivenName);

                    var message = await _messageRepository.GetMessageByIdAsync(request.Id);
                    if (message.Sender.UserName != userName && message.Recipient.UserName != userName)
                    {
                        res.Message = "Unauthorized, You are not allowed to delete this message";

                        return res;
                    }

                    if (message.Sender.UserName == userName)
                        message.SenderDeleted = true;

                    if (message.Recipient.UserName == userName)
                        message.RecipientDeleted = true;

                    await _messageRepository.UpdateAsync(message);

                    if (message.SenderDeleted && message.RecipientDeleted)
                        await _messageRepository.DeleteMessageAsync(message);

                    res.IsSuccess = true;
                    res.Message = "Message deleted successfully";

                    return res;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

    }
}
