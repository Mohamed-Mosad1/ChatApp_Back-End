using AutoMapper;
using ChatApp.Application.Features.Messages.Validator;
using ChatApp.Application.Persistence.Contracts;
using ChatApp.Application.Responses;
using ChatApp.Core.Entities;
using ChatApp.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

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
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly UserManager<AppUser> _userManager;
            private readonly IConfiguration _configuration;

            public Handler(
                IMapper mapper,
                IMessageRepository messageRepository,
                IHttpContextAccessor httpContextAccessor,
                UserManager<AppUser> userManager,
                IConfiguration configuration)
            {
                _messageRepository = messageRepository;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
                _userManager = userManager;
                _configuration = configuration;
            }

            public async Task<BaseCommonResponse> Handle(AddMessageCommand request, CancellationToken cancellationToken)
            {
                var response = new BaseCommonResponse();

                // Validate the request DTO
                var validationResult = await ValidateRequestAsync(request.AddMessageDto);
                if (!validationResult.IsSuccess)
                {
                    return validationResult;
                }

                // Map the DTO to the Message entity and set additional properties
                var message = _mapper.Map<Message>(request.AddMessageDto);
                var httpContext = _httpContextAccessor.HttpContext;

                message.SenderId = httpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                message.SenderUserName = httpContext?.User.FindFirstValue(ClaimTypes.GivenName) ?? "Unknown";

                // Get the recipient and sender details
                var sender = await GetUserWithPhotosAsync(message.SenderUserName);
                var recipient = await GetUserWithPhotosAsync(message.RecipientUserName);

                if (recipient is null)
                {
                    response.IsSuccess = false;
                    response.Message = "Recipient not found.";
                    return response;
                }

                // Prevent users from sending messages to themselves
                if (message.SenderUserName.ToLower() == request.AddMessageDto.RecipientUserName.ToLower())
                {
                    response.IsSuccess = false;
                    response.Message = "You cannot send a message to yourself.";
                    return response;
                }

                message.RecipientId = recipient.Id;
                message.RecipientUserName = recipient.UserName;

                // Save the message to the repository
                try
                {
                    await _messageRepository.AddAsync(message);
                    response.IsSuccess = true;
                    response.Message = "Message added successfully.";
                    response.Data = CreateResponseData(message, sender, recipient);
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.Message = $"An error occurred while adding the message: {ex.Message}";
                }

                return response;
            }

            private async Task<BaseCommonResponse> ValidateRequestAsync(AddMessageDto addMessageDto)
            {
                var response = new BaseCommonResponse();
                var validator = new MessageValidator();
                var validationResult = await validator.ValidateAsync(addMessageDto);

                if (!validationResult.IsValid)
                {
                    response.IsSuccess = false;
                    response.Message = "Validation errors occurred while adding a new message.";
                    response.Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                }
                else
                {
                    response.IsSuccess = true;
                }

                return response;
            }

            private async Task<AppUser?> GetUserWithPhotosAsync(string userName)
            {
                return await _userManager.Users.Include(u => u.Photos).FirstOrDefaultAsync(u => u.UserName == userName);
            }

            private object CreateResponseData(Message message, AppUser sender, AppUser recipient)
            {
                return new
                {
                    Id = message.Id,
                    SenderId = message.SenderId,
                    SenderUserName = message.SenderUserName,
                    SenderProfilePictureUrl = _configuration["BaseApiUrl"] + sender.Photos?.FirstOrDefault(p => p.IsMain)?.Url,
                    RecipientId = message.RecipientId,
                    RecipientUserName = message.RecipientUserName,
                    RecipientProfilePictureUrl = _configuration["BaseApiUrl"] + recipient.Photos?.FirstOrDefault(p => p.IsMain)?.Url,
                    Content = message.Content
                };
            }
        }
    }
}
