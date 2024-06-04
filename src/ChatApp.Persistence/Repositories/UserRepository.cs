using ChatApp.Application.Persistence.Contracts;
using ChatApp.Domain.Entities.Identity;
using ChatApp.Persistence.DatabaseContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;

        public UserRepository(ApplicationDbContext dbContext, UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<IReadOnlyList<AppUser>> GetAllUsersAsync()
        {
            return await _userManager.Users.Include(u => u.Photos).ToListAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(string userId)
        {
            return await _userManager.Users.Include(u => u.Photos).FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<AppUser> GetUserByUserNameAsync(string userName)
        {
            return await _userManager.Users.Include(u => u.Photos).FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task UpdateUser(AppUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                await _dbContext.SaveChangesAsync();

            else
                throw new Exception("Failed to update user");

        }
    }
}
