using ChatApp.Domain.Entities.Identity;

namespace ChatApp.Application.Persistence.Contracts
{
    public interface IUserRepository
    {
        Task UpdateUser(AppUser user);
        Task<IReadOnlyList<AppUser>> GetAllUsersAsync();
        Task<AppUser> GetUserByIdAsync(string userId);
        Task<AppUser> GetUserByUserNameAsync(string userName);
    }
}
