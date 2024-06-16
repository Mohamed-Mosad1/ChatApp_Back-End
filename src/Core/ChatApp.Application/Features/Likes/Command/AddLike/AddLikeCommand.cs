using ChatApp.Application.Persistence.Contracts;
using ChatApp.Application.Responses;
using ChatApp.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ChatApp.Application.Features.Likes.Command.AddLike
{
    public class AddLikeCommand : IRequest<BaseCommonResponse>
    {
        public string UserName { get; }

        public AddLikeCommand(string userName) => UserName = userName;

        public class Handler : IRequestHandler<AddLikeCommand, BaseCommonResponse>
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

            public async Task<BaseCommonResponse> Handle(AddLikeCommand request, CancellationToken cancellationToken)
            {
                var currentUserId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(currentUserId))
                    return new BaseCommonResponse { IsSuccess = false, Message = "Current user not found." };

                var sourceUser = await _userManager.FindByIdAsync(currentUserId);
                var destUser = await _userManager.FindByNameAsync(request.UserName);

                if (sourceUser is null || destUser is null)
                    return new BaseCommonResponse { IsSuccess = false, Message = "User not found." };

                if (sourceUser.Id == destUser.Id)
                    return new BaseCommonResponse { IsSuccess = false, Message = "You cannot like yourself." };

                if (await _userLikeRepository.GetUserLike(sourceUser.Id, destUser.Id) != null)
                    return new BaseCommonResponse { IsSuccess = false, Message = "You already liked this user." };


                var result = await _userLikeRepository.AddLike(destUser.Id, sourceUser.Id);

                return result
                    ? new BaseCommonResponse { IsSuccess = true, Message = "Like added successfully." }
                    : new BaseCommonResponse { IsSuccess = false, Message = "Failed to add like." };
            }
        }
    }
}
