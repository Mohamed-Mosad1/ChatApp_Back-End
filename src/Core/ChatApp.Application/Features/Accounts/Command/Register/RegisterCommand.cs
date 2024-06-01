using ChatApp.Application.Persistence.Contracts;
using ChatApp.Application.Responses;
using ChatApp.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.Application.Features.Accounts.Command.Register
{
    public class RegisterCommand : IRequest<BaseCommonResponse>
    {
        private readonly RegisterDto _registerDto;

        public RegisterCommand(RegisterDto registerDto)
        {
            _registerDto = registerDto;
        }

        class Handler : IRequestHandler<RegisterCommand, BaseCommonResponse>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly ITokenService _tokenService;

            public Handler(
                UserManager<AppUser> userManager,
                ITokenService tokenService
                )
            {
                _userManager = userManager;
                _tokenService = tokenService;
            }

            public async Task<BaseCommonResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
            {
                BaseCommonResponse res = new BaseCommonResponse();

                try
                {
                    var user = new AppUser()
                    {
                        Email = request._registerDto.Email,
                        UserName = request._registerDto.UserName
                    };

                    var response = await _userManager.CreateAsync(user, request._registerDto.Password);
                    if (!response.Succeeded)
                    {
                        foreach (var err in response.Errors)
                           res.Errors.Add($"{err.Code} - {err.Description}");
                        
                        res.IsSuccess = false;
                        res.Message = "BadRequest";
                        return res;
                    }

                    res.IsSuccess = true;
                    res.Message = "Register Success";
                    res.Data = new
                    {
                        userName = user.UserName,
                        email = user.Email,
                        token = await _tokenService.CreateTokenAsync(user)
                    };

                    return res;
                }
                catch (Exception ex)
                {
                    res.IsSuccess = false;
                    res.Message = ex.Message;

                    return res;
                }
            }
        }

    }
}
