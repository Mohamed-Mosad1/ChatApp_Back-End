using ChatApp.Application.Extensions;
using ChatApp.Application.Features.Likes.Command.AddOrRemoveLike;
using ChatApp.Application.Helpers;
using ChatApp.Application.Persistence.Contracts;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Entities.Identity;
using ChatApp.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ChatApp.Persistence.Repositories
{
    public class UserLikeRepository : IUserLikeRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public UserLikeRepository(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<UserLike?> GetUserLikeAsync(string sourceUserId, string likedUserId)
        {
            return await _dbContext.UserLikes.FindAsync(sourceUserId, likedUserId);
        }

        public async Task<PagedList<LikeDto>> GetUserLikesAsync(LikesParams likesParams)
        {
            var users = _dbContext.Users.Include(u => u.Photos).OrderBy(u => u.UserName).AsQueryable();
            var userLikes = _dbContext.UserLikes.AsQueryable();

            if (likesParams.Predicate == "liked")
            {
                userLikes = userLikes.Where(ul => ul.SourceUserId == likesParams.UserId);
                users = userLikes.Select(ul => ul.LikedUser);
            }

            else if (likesParams.Predicate == "likedBy")
            {
                userLikes = userLikes.Where(ul => ul.LikedUserId == likesParams.UserId);
                users = userLikes.Select(ul => ul.SourceUser);
            }

            var likedUser = users.Select(u => new LikeDto()
            {
                Id = u.Id,
                UserName = u.UserName,
                KnownAs = u.KnownAs,
                Age = u.DateOfBirth.CalculateAge(),
                City = u.City,
                IsLiked = _dbContext.UserLikes.Any(ul => ul.SourceUserId == likesParams.UserId && ul.LikedUserId == u.Id),
                PhotoUrl = _configuration["BaseApiUrl"] + u.Photos.FirstOrDefault(p => p.IsMain && p.IsActive).Url,
            });

            return await PagedList<LikeDto>.CreateAsync(likedUser, likesParams.PageNumber, likesParams.PageSize);
        }

        public async Task<AppUser?> GetUserWithLikesAsync(string userId)
        {
            return await _dbContext.Users.Include(u => u.LikeUser).FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<bool> AddLikeAsync(string likedUserId, string sourceUserId)
        {
            var userLike = new UserLike()
            {
                LikedUserId = likedUserId,
                SourceUserId = sourceUserId
            };

            await _dbContext.UserLikes.AddAsync(userLike);

            return true;
        }

        public async Task<bool> RemoveLikeAsync(string sourceUserId, string likedUserId)
        {
            var userLike = await _dbContext.UserLikes.FindAsync(sourceUserId, likedUserId);
            if (userLike is not null)
            {
                _dbContext.UserLikes.Remove(userLike);
                await _dbContext.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
