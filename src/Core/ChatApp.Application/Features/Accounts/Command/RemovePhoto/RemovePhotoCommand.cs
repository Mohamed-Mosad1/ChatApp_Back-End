using ChatApp.Application.Persistence.Contracts;
using MediatR;

namespace ChatApp.Application.Features.Accounts.Command.RemovePhoto
{
    public class RemovePhotoCommand : IRequest<bool>
    {
        public int PhotoId { get; set; }

        public RemovePhotoCommand(int photoId)
        {
            PhotoId = photoId;
        }

        class Handler : IRequestHandler<RemovePhotoCommand, bool>
        {
            private readonly IUserRepository _userRepository;

            public Handler(IUserRepository userRepository)
            {
                _userRepository = userRepository;
            }
            public async Task<bool> Handle(RemovePhotoCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    if (request.PhotoId > 0)
                    {
                        // Remove
                        await _userRepository.RemovePhotoAsync(request.PhotoId);
                        return true;
                    }

                    return false;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
    }
}
