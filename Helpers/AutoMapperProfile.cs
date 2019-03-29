using AutoMapper;
using trySample.Dtos;
using trySample.Entities;

namespace trySample.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
            CreateMap<UserResponseDto, User>();
            CreateMap<User, UserResponseDto>();
        }
    }
}