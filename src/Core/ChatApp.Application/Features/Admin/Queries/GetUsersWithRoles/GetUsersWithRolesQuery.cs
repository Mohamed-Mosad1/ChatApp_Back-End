using ChatApp.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Features.Admin.Queries.GetUsersWithRoles
{
    public class GetUsersWithRolesQuery : IRequest<List<UsersWithRolesDto>>
    {
        class Handler : IRequestHandler<GetUsersWithRolesQuery, List<UsersWithRolesDto>>
        {
            private readonly UserManager<AppUser> _userManager;

            public Handler(UserManager<AppUser> userManager)
            {
                _userManager = userManager;
            }

            public async Task<List<UsersWithRolesDto>> Handle(GetUsersWithRolesQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var users = await _userManager.Users.ToListAsync();
                    var userWithRoles = new List<UsersWithRolesDto>();

                    foreach (var user in users)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        var userDto = new UsersWithRolesDto()
                        {
                            Id = user.Id,
                            UserName = user.UserName,
                            Email = user.Email,
                            Roles = roles
                        };

                        userWithRoles.Add(userDto);
                    }

                    return userWithRoles;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }
    }
}
