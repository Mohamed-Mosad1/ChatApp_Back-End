using AutoMapper;
using ChatApp.Application.Persistence.Contracts;
using MediatR;

namespace ChatApp.Application.Features.Messages.Queries.GetAllMessages
{
    public class GetAllMessagesQuery : IRequest<List<MessageReturnDto>>
    {
        class Handler : IRequestHandler<GetAllMessagesQuery, List<MessageReturnDto>>
        {
            private readonly IMessageRepository _messageRepository;
            private readonly IMapper _mapper;

            public Handler(IMessageRepository messageRepository, IMapper mapper)
            {
                _messageRepository = messageRepository;
                _mapper = mapper;
            }

            public async Task<List<MessageReturnDto>> Handle(GetAllMessagesQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var allMessage = await _messageRepository.GetAllAsync();
                    var mappedMessage = _mapper.Map<List<MessageReturnDto>>(allMessage);

                    return mappedMessage;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
    }
}
