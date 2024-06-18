using AutoMapper;
using ChatApp.Application.Persistence.Contracts;
using ChatApp.Application.Responses;
using ChatApp.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ChatApp.Application.Features.Accounts.Command.Login
{
    public class LoginCommand : IRequest<BaseCommonResponse>
    {
        public LoginDto LoginDto { get; set; }

        public LoginCommand(LoginDto loginDto)
        {
            LoginDto = loginDto;
        }

        class Handler : IRequestHandler<LoginCommand, BaseCommonResponse>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly SignInManager<AppUser> _signInManager;
            private readonly ITokenService _tokenService;
            private readonly IMapper _mapper;
            private readonly IConfiguration _configuration;

            public Handler(
                UserManager<AppUser> userManager,
                SignInManager<AppUser> signInManager,
                ITokenService tokenService,
                IMapper mapper,
                IConfiguration configuration)
            {
                _userManager = userManager;
                _signInManager = signInManager;
                _tokenService = tokenService;
                _mapper = mapper;
                _configuration = configuration;
            }

            public async Task<BaseCommonResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
            {
                var response = new BaseCommonResponse();

                try
                {
                    var user = await _userManager.Users.Include(x=>x.Photos).FirstOrDefaultAsync(x=>x.UserName == request.LoginDto.UserName);

                    if (user == null)
                    {
                        response.Message = "User not found";
                        return response;
                    }

                    var result = await _signInManager.CheckPasswordSignInAsync(user, request.LoginDto.Password, false);

                    if (result.Succeeded)
                    {
                        response.IsSuccess = true;
                        response.Message = "Login successful";
                        response.Data = new
                        {
                            userName = user.UserName,
                            email = user.Email,
                            gender = user.Gender,
                            token = await _tokenService.CreateTokenAsync(user),
                            photoUrl = _configuration["BaseApiUrl"] + user.Photos.FirstOrDefault(p => p.IsMain && p.IsActive)?.Url
                        };
                    }
                    else
                    {
                        response.Message = "Unauthorized";
                    }

                    return response;
                }
                catch (Exception ex)
                {
                    response.Message = $"An error occurred: {ex.Message}";
                    return response;
                }
            }
        }
    }
}
