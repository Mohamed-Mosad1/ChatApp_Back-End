using ChatApp.Application.Persistence.Contracts;
using ChatApp.Application.Responses;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Entities.Identity;
using ChatApp.Persistence.Helpers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace ChatApp.Application.Features.Accounts.Command.SendResetPasswordEmail
{
    public class SendResetPasswordEmailCommand : IRequest<BaseCommonResponse>
    {
        public string Email { get; set; }

        public SendResetPasswordEmailCommand(string email)
        {
            Email = email;
        }
        class Handler : IRequestHandler<SendResetPasswordEmailCommand, BaseCommonResponse>
        {
            private readonly IUserRepository _userRepository;
            private readonly IConfiguration _configuration;
            private readonly IEmailService _emailService;
            private readonly UserManager<AppUser> _userManager;

            public Handler(
                IUserRepository userRepository,
                IConfiguration configuration,
                IEmailService emailService,
                UserManager<AppUser> userManager
                )
            {
                _userRepository = userRepository;
                _configuration = configuration;
                _emailService = emailService;
                _userManager = userManager;
            }
            public async Task<BaseCommonResponse> Handle(SendResetPasswordEmailCommand request, CancellationToken cancellationToken)
            {
                var response = new BaseCommonResponse();
                try
                {
                    var user = await _userRepository.GetUserByEmailAsync(request.Email);

                    if (user is null)
                    {
                        response.Message = "User Doesn't Exist";
                        return response;
                    }

                    var emailToken = await _userManager.GeneratePasswordResetTokenAsync(user);

                    string baseFrontUrl = _configuration["BaseFrontUrl"];
                    var emailModel = new EmailModel(request.Email, "Reset Password", EmailBody.GetEmailStringBody(request.Email, emailToken, baseFrontUrl));
                    await _emailService.SendEmailAsync(emailModel);
                    await _userRepository.UpdateUserAsync(user);

                    response.IsSuccess = true;
                    response.Message = "Email Sent Successfully";

                    return response;
                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;

                    return response;
                }
            }
        }
    }
}
