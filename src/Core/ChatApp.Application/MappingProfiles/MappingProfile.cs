using AutoMapper;
using ChatApp.Application.Features.Messages.Command.AddMessage;
using ChatApp.Core.Entities;

namespace ChatApp.Application.MappingProfiles
{
    internal class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapping Message
            CreateMap<Message, AddMessageDto>().ReverseMap();



        }
    }
}
