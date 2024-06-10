using ChatApp.Application.Features.Accounts.Queries.GetAllUsers;
using ChatApp.Application.Helpers;
using ChatApp.Domain.Entities.Identity;
using ChatApp.Persistence.Helpers;
using Microsoft.AspNetCore.Http;

namespace ChatApp.Application.Persistence.Contracts
{
    public interface IUserRepository
    {
        Task UpdateUserAsync(AppUser user);
        Task<IReadOnlyList<AppUser>> GetAllUsersAsync();
        Task<AppUser?> GetUserByIdAsync(string userId);
        Task<AppUser?> GetUserByUserNameAsync(string userName);
        Task<PhotoDto?> UploadPhotoAsync(IFormFile file, string pathName);
        Task<bool> RemovePhotoAsync(int photoId);
        Task<bool> SetMainPhotoAsync(int photoId);
        Task<PagedList<MemberDto>> GetAllMembersAsync(UserParams userParams);
    }
}
