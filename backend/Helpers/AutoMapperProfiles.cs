using AutoMapper;
using backend.DTOs;
using backend.Entities;

namespace backend.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<UserDto, User>().ReverseMap();
        }
    }
}
