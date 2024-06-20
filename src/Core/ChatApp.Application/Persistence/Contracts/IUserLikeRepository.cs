using ChatApp.Application.Features.Likes.Command.AddOrRemoveLike;
using ChatApp.Application.Helpers;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Entities.Identity;

namespace ChatApp.Application.Persistence.Contracts
{
    public interface IUserLikeRepository
    {
        Task<PagedList<LikeDto>> GetUserLikesAsync(LikesParams likesParams);
        Task<UserLike?> GetUserLikeAsync(string sourceUserId, string likedUserId);
        Task<AppUser?> GetUserWithLikesAsync(string userId);
        Task<bool> AddLikeAsync(string likedUserId, string sourceUserId);
        Task<bool> RemoveLikeAsync(string likedUserId, string sourceUserId);

    }
}
