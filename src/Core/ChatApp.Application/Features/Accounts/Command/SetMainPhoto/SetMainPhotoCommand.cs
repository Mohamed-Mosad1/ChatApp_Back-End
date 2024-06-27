using ChatApp.Application.Persistence.Contracts;
using ChatApp.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.Application.Features.Accounts.Command.SetMainPhoto
{
    public class SetMainPhotoCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public SetMainPhotoCommand(int id)
        {
            Id = id;
        }

        class Handler : IRequestHandler<SetMainPhotoCommand, bool>
        {
            private readonly IUserRepository _userRepository;
            private readonly UserManager<AppUser> _userManager;

            public Handler(IUserRepository userRepository, UserManager<AppUser> userManager)
            {
                _userRepository = userRepository;
                _userManager = userManager;
            }

            public async Task<bool> Handle(SetMainPhotoCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    if (request.Id > 0)
                    {
                        var result = await _userRepository.SetMainPhotoAsync(request.Id);
                        if (result)
                            return true;
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
