using AutoMapper;
using TheMathAndScienceAcademy.Application.Features.Students.Dtos;
using TheMathAndScienceAcademy.Domain.Entities;

namespace TheMathAndScienceAcademy.Application.Mapping.Profile;

public class StudentProfile : AutoMapper.Profile
{
    public StudentProfile()
    {
        CreateMap<Student, StudentDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)))
            .ForMember(dest => dest.AcademyId, opt => opt.MapFrom(src => Guid.Parse(src.AcademyId)))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId == null ? (Guid?)null : Guid.Parse(src.UserId)));
    }
}
