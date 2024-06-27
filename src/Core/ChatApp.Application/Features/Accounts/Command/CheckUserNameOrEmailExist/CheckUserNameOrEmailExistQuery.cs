using ChatApp.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.Application.Features.Accounts.Command.CheckUserNameOrEmailExist
{
    public class CheckUserNameOrEmailExistQuery : IRequest<bool>
    {
        public string SearchTerm { get; set; }

        public CheckUserNameOrEmailExistQuery(string searchTerm)
        {
            SearchTerm = searchTerm;
        }

        class Handler : IRequestHandler<CheckUserNameOrEmailExistQuery, bool>
        {
            private readonly UserManager<AppUser> _userManager;

            public Handler(UserManager<AppUser> userManager)
            {
                _userManager = userManager;
            }

            public async Task<bool> Handle(CheckUserNameOrEmailExistQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    if (!string.IsNullOrEmpty(request.SearchTerm))
                    {
                        if (request.SearchTerm.Contains("@"))
                        {
                            // Email
                            var email = await _userManager.FindByEmailAsync(request.SearchTerm);
                            if (email is not null)
                                return true;
                        }
                        else
                        {
                            // UserName
                            var userName = await _userManager.FindByNameAsync(request.SearchTerm);
                            if (userName is not null)
                                return true;
                        }
                    }

                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }
}
