using AutoMapper;
using TheMathAndScienceAcademy.Application.Features.Classes.Dtos;
using TheMathAndScienceAcademy.Domain.Entities;

namespace TheMathAndScienceAcademy.Application.Mapping.Profile;

public class AcademicClassProfile : AutoMapper.Profile
{
    public AcademicClassProfile() => CreateMap<AcademicClass, AcademicClassDto>()
        .ForMember(x => x.Id, o => o.MapFrom(x => Guid.Parse(x.Id)))
        .ForMember(x => x.AcademyId, o => o.MapFrom(x => Guid.Parse(x.AcademyId)));
}
