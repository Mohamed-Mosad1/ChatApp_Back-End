using ChatApp.Application.Features.Likes.Command.AddLike;
using ChatApp.Application.Helpers;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Entities.Identity;

namespace ChatApp.Application.Persistence.Contracts
{
    public interface IUserLikeRepository
    {
        Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams);
        Task<UserLike?> GetUserLike(string sourceUserId, string likedUserId);
        Task<AppUser?> GetUserWithLikes(string userId);
        Task<bool> AddLike(string likedUserId, string sourceUserId);

    }
}
