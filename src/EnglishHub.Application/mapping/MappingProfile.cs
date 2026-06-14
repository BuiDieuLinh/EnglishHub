using AutoMapper;
using EnglishHub.Application.DTOs;
using EnglishHub.Domain.Entities;

namespace EnglishHub.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Users, UserResponse>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));
    }
}