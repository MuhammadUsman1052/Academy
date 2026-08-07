using AutoMapper;
using TheMathAndScienceAcademy.Application.Common;
using TheMathAndScienceAcademy.Application.Features.RolePermissions.Dtos;
using TheMathAndScienceAcademy.Domain.Entities;

namespace TheMathAndScienceAcademy.Application.Mapping.Profile;

public class RolePermissionProfile : AutoMapper.Profile
{
    public RolePermissionProfile()
    {
        CreateMap<RolePermission, RolePermissionDto>()
            .ForMember(dest => dest.PermissionId, opt => opt.MapFrom(src => Guid.Parse(src.PermissionId)))
            .ForMember(dest => dest.PermissionName, opt => opt.MapFrom(src => src.Permission.Name))
            .ForMember(dest => dest.PermissionDescription, opt => opt.MapFrom(src => src.Permission.Description))
            .ForMember(dest => dest.ModuleName, opt => opt.MapFrom(src => PermissionNameParser.GetModuleName(src.Permission.Name)))
            .ForMember(dest => dest.IsGranted, opt => opt.MapFrom(src => src.IsGranted));
    }
}
