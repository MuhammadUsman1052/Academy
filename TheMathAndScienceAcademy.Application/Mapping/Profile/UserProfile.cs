using AutoMapper;
using TheMathAndScienceAcademy.Application.Features.Users.Dtos;
using TheMathAndScienceAcademy.Domain.Entities;

namespace TheMathAndScienceAcademy.Application.Mapping.Profile;

public class UserProfile : AutoMapper.Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)))
            .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => Guid.Parse(src.RoleId)))
            .ForMember(dest => dest.AcademyId, opt => opt.MapFrom(src => src.AcademyId == null ? (Guid?)null : Guid.Parse(src.AcademyId)))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));
    }
}
