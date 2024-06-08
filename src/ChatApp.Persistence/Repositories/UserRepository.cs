using ChatApp.Application.Persistence.Contracts;
using ChatApp.Domain.Entities.Identity;
using ChatApp.Persistence.DatabaseContext;
using ChatApp.Persistence.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ChatApp.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _webHost;
        private readonly IHttpContextAccessor _httpContext;

        public UserRepository(
            ApplicationDbContext dbContext,
            UserManager<AppUser> userManager,
            IWebHostEnvironment webHost,
            IHttpContextAccessor httpContext
            )
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _webHost = webHost;
            _httpContext = httpContext;
        }

        public async Task<IReadOnlyList<AppUser>> GetAllUsersAsync()
        {
            return await _userManager.Users.Include(u => u.Photos).ToListAsync();
        }

        public async Task<AppUser?> GetUserByIdAsync(string userId)
        {
            return await _userManager.Users.Include(u => u.Photos).FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<AppUser?> GetUserByUserNameAsync(string userName)
        {
            return await _userManager.Users.Include(u => u.Photos).FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task UpdateUserAsync(AppUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                await _dbContext.SaveChangesAsync();

            else
                throw new Exception("Failed to update user");

        }

        public async Task<string> UploadPhotoAsync(IFormFile file, string pathName)
        {
            if (file == null)
            {
                throw new ArgumentException("File cannot be null", nameof(file));
            }

            var userName = _httpContext.HttpContext?.User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.GivenName)?.Value;
            if (userName == null)
            {
                throw new Exception("User not found");
            }

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var photoUrl = await PhotoManager.UploadPhotoAsync(_webHost, file, pathName);
            var photo = new Photo
            {
                Url = photoUrl,
                AppUserId = user.Id,
                IsActive = true
            };

            await _dbContext.Photos.AddAsync(photo);
            await _dbContext.SaveChangesAsync();

            return photoUrl;
        }


        public async Task<bool> RemovePhotoAsync(int photoId)
        {
            var currentPhoto = await _dbContext.Photos.FirstOrDefaultAsync(p => p.Id == photoId);
            if (currentPhoto is not null)
            {
                _dbContext.Photos.Remove(currentPhoto);
                await _dbContext.SaveChangesAsync();

                PhotoManager.RemovePhoto(_webHost, currentPhoto.Url);
                return true;
            }

            return false;
        }

        public async Task<bool> SetMainPhotoAsync(int photoId)
        {
            var userName = _httpContext?.HttpContext?.User.Claims
                              .FirstOrDefault(u => u.Type == ClaimTypes.GivenName)?.Value;

            if (userName == null) return false;

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null) return false;

            var currentPhotos = await _dbContext.Photos
                                   .Where(p => p.AppUserId == user.Id).ToListAsync();

            foreach (var photo in currentPhotos)
            {
                photo.IsMain = photo.Id == photoId;
            }

            await _dbContext.SaveChangesAsync();

            return currentPhotos.Any(p => p.Id == photoId && p.IsMain);
        }

    }
}
