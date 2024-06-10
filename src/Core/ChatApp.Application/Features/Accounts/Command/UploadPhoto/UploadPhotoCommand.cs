using ChatApp.Application.Persistence.Contracts;
using ChatApp.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.Application.Features.Accounts.Command.UploadPhoto
{
    public class UploadPhotoCommand : IRequest<PhotoDto>
    {
        public IFormFile? PhotoFile { get; set; }
        class Handler : IRequestHandler<UploadPhotoCommand, PhotoDto?>
        {
            private readonly IUserRepository _userRepository;
            private readonly IHttpContextAccessor _httpContext;
            private readonly UserManager<AppUser> _userManager;

            public Handler(IUserRepository userRepository, IHttpContextAccessor httpContext, UserManager<AppUser> userManager)
            {
                _userRepository = userRepository;
                _httpContext = httpContext;
                _userManager = userManager;
            }

            public async Task<PhotoDto?> Handle(UploadPhotoCommand request, CancellationToken cancellationToken)
            {
                if (request.PhotoFile is not null)
                {
                    var result = await _userRepository.UploadPhotoAsync(request.PhotoFile, "User");
                    if (result is not null)
                    {
                        return result;
                    }

                    return null;
                }

                return null;
            }
        }
    }
}
