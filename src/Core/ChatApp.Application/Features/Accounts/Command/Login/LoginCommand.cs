using AutoMapper;
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
            private readonly RoleManager<IdentityRole> _roleManager;
            private readonly SignInManager<AppUser> _signInManager;
            private readonly ITokenService _tokenService;
            private readonly IMapper _mapper;

            public Handler(
                UserManager<AppUser> userManager,
                RoleManager<IdentityRole> roleManager,
                SignInManager<AppUser> signInManager,
                ITokenService tokenService,
                IMapper mapper)
            {
                _userManager = userManager;
                _roleManager = roleManager;
                _signInManager = signInManager;
                _tokenService = tokenService;
                _mapper = mapper;
            }

            public async Task<BaseCommonResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
            {
                BaseCommonResponse res = new BaseCommonResponse();
                try
                {
                    var user = await _userManager.FindByNameAsync(request.LoginDto.UserName);

                    if (user is not null)
                    {
                        var result = await _signInManager.CheckPasswordSignInAsync(user, request.LoginDto.Password, false);
                        if (request is not null && result.Succeeded)
                        {
                            res.IsSuccess = true;
                            res.Message = "Login Success";
                            res.Data = new
                            {
                                userName = user.UserName,
                                email = user.Email,
                                token = await _tokenService.CreateTokenAsync(user)
                            };

                            return res;
                        }
                        res.IsSuccess = false;
                        res.Message = "UnAuthorized";
                        return res;
                    }

                    res.IsSuccess = false;
                    res.Message = "NotFound";
                    return res;
                }
                catch (Exception ex)
                {
                    res.IsSuccess = false;
                    res.Message = ex.InnerException.Message;
                    return res;
                }
            }
        }
    }
}
