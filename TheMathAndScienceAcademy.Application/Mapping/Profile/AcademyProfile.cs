using AutoMapper;
using TheMathAndScienceAcademy.Application.Features.Academies.Command.Models;
using TheMathAndScienceAcademy.Application.Features.Academies.Dtos;
using TheMathAndScienceAcademy.Domain.Entities;

namespace TheMathAndScienceAcademy.Application.Mapping.Profile;

public class AcademyProfile : AutoMapper.Profile
{
    public AcademyProfile()
    {
        CreateMap<CreateAcademyCommand, Academy>();

        CreateMap<UpdateAcademyCommand, Academy>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        CreateMap<Academy, AcademyDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)));
    }
}
