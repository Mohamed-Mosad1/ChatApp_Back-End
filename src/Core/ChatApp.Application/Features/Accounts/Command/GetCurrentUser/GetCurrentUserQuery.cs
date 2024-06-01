using ChatApp.Application.Persistence.Contracts;
using ChatApp.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Features.Accounts.Command.GetCurrentUser
{
    public class GetCurrentUserQuery : IRequest<UserToReturnDto>
    {
        class Handler : IRequestHandler<GetCurrentUserQuery, UserToReturnDto>
        {
            private readonly IHttpContextAccessor _httpContext;
            private readonly UserManager<AppUser> _userManager;
            private readonly ITokenService _tokenService;

            public Handler(
                IHttpContextAccessor httpContext,
                UserManager<AppUser> userManager,
                ITokenService tokenService)
            {
                _httpContext = httpContext;
                _userManager = userManager;
                _tokenService = tokenService;
            }
            public async Task<UserToReturnDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var userName = _httpContext.HttpContext.User.FindFirstValue(ClaimTypes.GivenName);
                    if (userName is not null)
                    {
                        var user = await _userManager.FindByNameAsync(userName);

                        return new UserToReturnDto()
                        {
                            UserId = user.Id,
                            Email = user.Email,
                            UserName = user.UserName,
                            Token = await _tokenService.CreateTokenAsync(user)
                        };
                    }

                    return null;

                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }
}
