using AutoMapper;
using TheMathAndScienceAcademy.Application.Features.Permissions.Command.Models;
using TheMathAndScienceAcademy.Application.Features.Permissions.Dtos;
using TheMathAndScienceAcademy.Domain.Entities;

namespace TheMathAndScienceAcademy.Application.Mapping.Profile;

public class PermissionProfile : AutoMapper.Profile
{
    public PermissionProfile()
    {
        CreateMap<CreatePermissionCommand, Permission>();
        CreateMap<UpdatePermissionCommand, Permission>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<Permission, PermissionDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)));
    }
}
