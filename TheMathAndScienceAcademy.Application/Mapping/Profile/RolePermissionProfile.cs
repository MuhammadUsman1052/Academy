using AutoMapper;
using TheMathAndScienceAcademy.Application.Features.RolePermissions.Dtos;
using TheMathAndScienceAcademy.Domain.Entities;

namespace TheMathAndScienceAcademy.Application.Mapping.Profile;

public class RolePermissionProfile : AutoMapper.Profile
{
    public RolePermissionProfile()
    {
        CreateMap<Permission, RolePermissionDto>()
            .ForMember(dest => dest.PermissionId, opt => opt.MapFrom(src => Guid.Parse(src.Id)))
            .ForMember(dest => dest.PermissionName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.PermissionDescription, opt => opt.MapFrom(src => src.Description));
    }
}
