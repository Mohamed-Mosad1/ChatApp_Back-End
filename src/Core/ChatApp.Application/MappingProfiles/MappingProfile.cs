using AutoMapper;
using ChatApp.Application.Extensions;
using ChatApp.Application.Features.Accounts.Command.Register;
using ChatApp.Application.Features.Accounts.Queries.GetAllUsers;
using ChatApp.Application.Features.Messages.Command.AddMessage;
using ChatApp.Application.Features.Messages.Query.GetAllMessages;
using ChatApp.Application.Helpers;
using ChatApp.Core.Entities;
using ChatApp.Domain.Entities.Identity;

namespace ChatApp.Application.MappingProfiles
{
    public class MappingProfile : Profile
    {
        private const string baseUrl = "https://localhost:44364/";

        public MappingProfile()
        {
            // Mapping Message with AddMessageDto and MessageReturnDto
            CreateMap<Message, AddMessageDto>().ReverseMap();

            CreateMap<Message, MessageReturnDto>().ReverseMap();

            // Mapping User with Photos
            CreateMap<AppUser, MemberDto>()
                .ForMember(d => d.PhotoUrl, opt => opt.MapFrom(s=>baseUrl + s.Photos.FirstOrDefault(x=>x.IsMain).Url))
                //.ForMember(d => d.PhotoUrl, opt => opt.MapFrom<PhotoMemberResolver>())
                .ForMember(d => d.Age, opt => opt.MapFrom(s => s.DateOfBirth.CalculateAge()))
                .ReverseMap();

            // Mapping Photo with PhotoDto
            CreateMap<Photo, PhotoDto>()
                .ForMember(d=>d.Url, opt=>opt.MapFrom<UserPhotoResolver>())
                .ReverseMap();

            // Mapping AppUser with RegisterDto
            CreateMap<AppUser, RegisterDto>().ReverseMap();

        }
    }
}
