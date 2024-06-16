using ChatApp.Application.Features.Likes.Command.AddLike;
using ChatApp.Application.Helpers;
using ChatApp.Application.Persistence.Contracts;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ChatApp.Application.Features.Likes.Query.GetLikedUsers
{
    public class GetUserLikesQuery : IRequest<PagedList<LikeDto>>
    {
        public LikesParams LikesParams { get; }

        public GetUserLikesQuery(LikesParams likesParams)
        {
            LikesParams = likesParams;
        }

        class Handler : IRequestHandler<GetUserLikesQuery, PagedList<LikeDto>>
        {
            private readonly IUserLikeRepository _userLikeRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IUserLikeRepository userLikeRepository, IHttpContextAccessor httpContextAccessor)
            {
                _userLikeRepository = userLikeRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<PagedList<LikeDto>> Handle(GetUserLikesQuery request, CancellationToken cancellationToken)
            {
                var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    throw new UnauthorizedAccessException("User not authenticated");
                }

                request.LikesParams.UserId = userId;
                var userLikes = await _userLikeRepository.GetUserLikes(request.LikesParams);

                return userLikes ?? new PagedList<LikeDto>(new List<LikeDto>(), 0, 1, request.LikesParams.PageSize);
            }
        }
    }
}
