using AutoMapper;
using ChatApp.Application.Features.Accounts.Queries.GetAllUsers;
using ChatApp.Domain.Entities.Identity;
using Microsoft.Extensions.Configuration;

namespace ChatApp.Application.Helpers
{
    public class PhotoMemberResolver : IValueResolver<AppUser, MemberDto, string>
    {
        private readonly IConfiguration _configuration;

        public PhotoMemberResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Resolve(AppUser source, MemberDto destination, string destMember, ResolutionContext context)
        {
            return _configuration["BaseApiUrl"] + source.Photos.FirstOrDefault(p=>p.IsMain && p.IsActive)?.Url;

        }
    }
}
