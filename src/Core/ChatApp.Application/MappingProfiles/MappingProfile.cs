using AutoMapper;
using ChatApp.Application.Extensions;
using ChatApp.Application.Features.Accounts.Queries.GetAllUsers;
using ChatApp.Application.Features.Messages.Command.AddMessage;
using ChatApp.Application.Features.Messages.Query.GetAllMessages;
using ChatApp.Core.Entities;
using ChatApp.Domain.Entities.Identity;

namespace ChatApp.Application.MappingProfiles
{
    internal class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapping Message
            CreateMap<Message, AddMessageDto>().ReverseMap();

            CreateMap<Message, MessageReturnDto>().ReverseMap();

            // Mapping User and Photos
            CreateMap<AppUser, MemberDto>()
                .ForMember(d => d.PhotoUrl, opt => opt.MapFrom(s => s.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(d => d.Age, opt => opt.MapFrom(s => s.DateOfBirth.CalculateAge()))
                .ReverseMap();
            CreateMap<Photo, PhotoDto>().ReverseMap();



        }
    }
}
