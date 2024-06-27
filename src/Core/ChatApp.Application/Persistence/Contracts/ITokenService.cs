using ChatApp.Domain.Entities.Identity;

namespace ChatApp.Application.Persistence.Contracts
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(AppUser user);
    }
}
