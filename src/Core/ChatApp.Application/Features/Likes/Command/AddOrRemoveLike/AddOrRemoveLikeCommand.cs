using ChatApp.Application.Persistence.Contracts;
using ChatApp.Application.Responses;
using ChatApp.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ChatApp.Application.Features.Likes.Command.AddOrRemoveLike
{
    public class AddOrRemoveLikeCommand : IRequest<BaseCommonResponse>
    {
        public string UserName { get; }

        public AddOrRemoveLikeCommand(string userName) => UserName = userName;

        public class Handler : IRequestHandler<AddOrRemoveLikeCommand, BaseCommonResponse>
        {
            private readonly IUserLikeRepository _userLikeRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly UserManager<AppUser> _userManager;

            public Handler(IUserLikeRepository userLikeRepository, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager)
            {
                _userLikeRepository = userLikeRepository;
                _httpContextAccessor = httpContextAccessor;
                _userManager = userManager;
            }

            public async Task<BaseCommonResponse> Handle(AddOrRemoveLikeCommand request, CancellationToken cancellationToken)
            {
                var currentUserId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(currentUserId))
                    return new BaseCommonResponse { Message = "Current user not found." };

                var sourceUser = await _userManager.FindByIdAsync(currentUserId);
                var destUser = await _userManager.FindByNameAsync(request.UserName);

                if (sourceUser is null || destUser is null)
                    return new BaseCommonResponse { Message = "User not found." };

                if (sourceUser.Id == destUser.Id)
                    return new BaseCommonResponse { Message = "You cannot like yourself." };

                if (await _userLikeRepository.GetUserLikeAsync(sourceUser.Id, destUser.Id) is not null)
                {
                    await _userLikeRepository.RemoveLikeAsync(sourceUser.Id, destUser.Id);
                    return new BaseCommonResponse { IsSuccess = true, Message = "You have unliked " };
                }


                var result = await _userLikeRepository.AddLikeAsync(destUser.Id, sourceUser.Id);

                return result
                    ? new BaseCommonResponse { IsSuccess = true, Message = "You have liked " }
                    : new BaseCommonResponse { Message = "Failed to add like." };
            }
        }
    }
}
