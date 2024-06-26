using ChatApp.Application.Persistence.Contracts;
using ChatApp.Application.Responses;
using ChatApp.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Features.Accounts.Command.ResetPassword
{
    public class ResetPasswordCommand : IRequest<BaseCommonResponse>
    {
        public ResetPasswordDto ResetPasswordDto { get; set; }

        public ResetPasswordCommand(ResetPasswordDto resetPasswordDto)
        {
            ResetPasswordDto = resetPasswordDto;
        }

        class Handler : IRequestHandler<ResetPasswordCommand, BaseCommonResponse>
        {
            private readonly IUserRepository _userRepository;
            private readonly UserManager<AppUser> _userManager;

            public Handler(IUserRepository userRepository, UserManager<AppUser> userManager)
            {
                _userRepository = userRepository;
                _userManager = userManager;
            }
            public async Task<BaseCommonResponse> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
            {
                var response = new BaseCommonResponse();
                try
                {
                    var user = await _userRepository.GetUserByEmailAsync(request.ResetPasswordDto.Email);
                    if (user is null)
                    {
                        response.Message = "User Doesn't Exist";
                        return response;
                    }

                    // Update user's password here
                    var resetPasswordResult = await _userManager.ResetPasswordAsync(user, request.ResetPasswordDto.EmailToken, request.ResetPasswordDto.NewPassword);
                    var errorCode = resetPasswordResult.Errors.FirstOrDefault(x => x.Code == "InvalidToken");
                    if (errorCode != null)
                    {
                        response.Message = "Expired reset link try reset your password again";

                        return response;
                    }
                    
                    if (!resetPasswordResult.Succeeded)
                    {
                        response.Message = "Password reset failed";
                        response.Errors = resetPasswordResult.Errors.Select(e => e.Description).ToList();
                        return response;
                    }

                    await _userRepository.UpdateUserAsync(user);

                    response.IsSuccess = true;
                    response.Message = "Password Reset Successfully";
                    
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
