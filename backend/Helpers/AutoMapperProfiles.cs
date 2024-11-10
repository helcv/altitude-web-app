using AutoMapper;
using backend.DTOs;
using backend.Entities;

namespace backend.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.IsVerified, opt => opt.MapFrom(src => src.EmailConfirmed))
                .ReverseMap();
        }
    }
}
