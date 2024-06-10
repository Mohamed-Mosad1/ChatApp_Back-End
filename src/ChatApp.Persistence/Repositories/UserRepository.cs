using AutoMapper;
using AutoMapper.QueryableExtensions;
using ChatApp.Application.Features.Accounts.Queries.GetAllUsers;
using ChatApp.Application.Helpers;
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
        private readonly IMapper _mapper;

        public UserRepository(
            ApplicationDbContext dbContext,
            UserManager<AppUser> userManager,
            IWebHostEnvironment webHost,
            IHttpContextAccessor httpContext,
            IMapper mapper
            )
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _webHost = webHost;
            _httpContext = httpContext;
            _mapper = mapper;
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

        public async Task<PhotoDto?> UploadPhotoAsync(IFormFile file, string pathName)
        {
            if (file is null)
            {
                throw new ArgumentException("File cannot be null", nameof(file));
            }

            var userName = _httpContext.HttpContext?.User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.GivenName)?.Value;
            if (userName is not null)
            {
                var user = await _userManager.FindByNameAsync(userName);
                if (user is not null)
                {
                    var photoUrl = await PhotoManager.UploadPhotoAsync(_webHost, file, pathName);
                    var photo = new Photo()
                    {
                        Url = photoUrl,
                        AppUserId = user.Id,
                        IsActive = true
                    };

                    await _dbContext.Photos.AddAsync(photo);
                    await _dbContext.SaveChangesAsync();

                    var mappedPhoto = _mapper.Map<PhotoDto>(photo);

                    return mappedPhoto;
                }
            }

            return null;
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

        public async Task<PagedList<MemberDto>> GetAllMembersAsync(UserParams userParams)
        {
            var query = _dbContext.Users.Include(x => x.Photos).AsQueryable();

            // Apply filters
            query = query.Where(x => x.UserName != userParams.CurrentUserName);

            if (!string.IsNullOrEmpty(userParams.Gender))
            {
                query = query.Where(x => x.Gender == userParams.Gender);
            }

            if (userParams.MinAge > 0 || userParams.MaxAge < int.MaxValue)
            {
                var maxDob = DateTime.Today.AddYears(-userParams.MinAge);
                var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);

                query = query.Where(x => x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob);
            }

            // Sorting by last active
            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(x => x.Created),
                _ => query.OrderByDescending(x => x.LastActive)
            };

            var result = query.ProjectTo<MemberDto>(_mapper.ConfigurationProvider);

            return await PagedList<MemberDto>.CreateAsync(result, userParams.PageNumber, userParams.PageSize);
        }

    }
}
