using AutoMapper;
using ChatApp.Application.Features.Messages.Validator;
using ChatApp.Application.Persistence.Contracts;
using ChatApp.Application.Responses;
using ChatApp.Core.Entities;
using MediatR;

namespace ChatApp.Application.Features.Messages.Command.AddMessage
{
    public class AddMessageCommand : IRequest<BaseCommonResponse>
    {
        public AddMessageDto AddMessageDto { get; set; }

        public AddMessageCommand(AddMessageDto addMessageDto)
        {
            AddMessageDto = addMessageDto;
        }

        class Handler : IRequestHandler<AddMessageCommand, BaseCommonResponse>
        {
            private readonly IMessageRepository _messageRepository;
            private readonly IMapper _mapper;

            public Handler(IMessageRepository messageRepository, IMapper mapper)
            {
                _messageRepository = messageRepository;
                _mapper = mapper;
            }

            public async Task<BaseCommonResponse> Handle(AddMessageCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    BaseCommonResponse response = new BaseCommonResponse();
                    MessageValidator validations = new MessageValidator();

                    var validatorResult = await validations.ValidateAsync(request.AddMessageDto);
                    if (!validatorResult.IsValid)
                    {
                        response.IsSuccess = false;
                        response.Message = "While Adding New Message";
                        response.Errors = validatorResult.Errors.Select(x => x.ErrorMessage).ToList();
                    }

                    var mappedMessage = _mapper.Map<Message>(request.AddMessageDto);
                    response.IsSuccess = true;
                    response.Message = "Success Adding New Message";
                    await _messageRepository.AddAsync(mappedMessage);

                    return response;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
    }
}
