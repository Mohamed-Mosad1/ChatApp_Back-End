using ChatApp.Application.Extensions;
using ChatApp.Application.Features.Likes.Command.AddLike;
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

        public async Task<UserLike?> GetUserLike(string sourceUserId, string likedUserId)
        {
            return await _dbContext.UserLikes.FindAsync(sourceUserId, likedUserId);
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
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
                Age = u.DateOfBirth.CalculateAge(),
                City = u.City,
                KnownAs = u.KnownAs,
                PhotoUrl = _configuration["BaseApiUrl"] + u.Photos.FirstOrDefault(p => p.IsMain && p.IsActive).Url,
                UserName = u.UserName
            });

            return await PagedList<LikeDto>.CreateAsync(likedUser, likesParams.PageNumber, likesParams.PageSize);
        }

        public async Task<AppUser?> GetUserWithLikes(string userId)
        {
            return await _dbContext.Users.Include(u => u.LikeUser).FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<bool> AddLike(string likedUserId, string sourceUserId)
        {
            var userLike = new UserLike()
            {
                LikedUserId = likedUserId,
                SourceUserId = sourceUserId
            };

            await _dbContext.UserLikes.AddAsync(userLike);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
