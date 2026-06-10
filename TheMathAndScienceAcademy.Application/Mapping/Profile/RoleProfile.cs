using AutoMapper;
using TheMathAndScienceAcademy.Application.Features.Roles.Command.Models;
using TheMathAndScienceAcademy.Application.Features.Roles.Dtos;
using TheMathAndScienceAcademy.Domain.Entities;

namespace TheMathAndScienceAcademy.Application.Mapping.Profile;

public class RoleProfile : AutoMapper.Profile
{
    public RoleProfile()
    {
        CreateMap<CreateRoleCommand, Role>();
        CreateMap<UpdateRoleCommand, Role>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<Role, RoleDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)));
    }
}
