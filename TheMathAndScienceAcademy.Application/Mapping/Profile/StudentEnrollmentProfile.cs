using AutoMapper;
using TheMathAndScienceAcademy.Application.Features.StudentEnrollments.Dtos;
using TheMathAndScienceAcademy.Domain.Entities;

namespace TheMathAndScienceAcademy.Application.Mapping.Profile;

public class StudentEnrollmentProfile : AutoMapper.Profile
{
    public StudentEnrollmentProfile() => CreateMap<StudentEnrollment, StudentEnrollmentDto>().ForMember(x => x.Id, o => o.MapFrom(x => Guid.Parse(x.Id))).ForMember(x => x.StudentId, o => o.MapFrom(x => Guid.Parse(x.StudentId))).ForMember(x => x.ClassId, o => o.MapFrom(x => Guid.Parse(x.ClassId))).ForMember(x => x.AcademyId, o => o.MapFrom(x => Guid.Parse(x.AcademyId)));
}
