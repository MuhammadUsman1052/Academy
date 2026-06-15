using AutoMapper;
using TheMathAndScienceAcademy.Application.Features.Auth.Dtos;
using TheMathAndScienceAcademy.Domain.Entities;

namespace TheMathAndScienceAcademy.Application.Mapping.Profile;

public class AuthProfile : AutoMapper.Profile
{
    public AuthProfile()
    {
        CreateMap<User, LoginResponseDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));

        CreateMap<User, CurrentUserDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));
    }
}
