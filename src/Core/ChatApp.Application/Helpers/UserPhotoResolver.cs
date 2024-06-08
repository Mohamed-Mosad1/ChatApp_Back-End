using AutoMapper;
using ChatApp.Domain.Entities.Identity;
using Microsoft.Extensions.Configuration;

namespace ChatApp.Application.Helpers
{
    public class UserPhotoResolver : IValueResolver<Photo, PhotoDto, string?>
    {
        private readonly IConfiguration _configuration;

        public UserPhotoResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string? Resolve(Photo source, PhotoDto destination, string? destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.Url))
            {
                return _configuration["BaseApiUrl"] + source.Url;
            }

            return null;
        }
    }
}
