using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Features.Students.Dtos;

namespace TheMathAndScienceAcademy.Application.Features.Students.Query.Models;

public class GetStudentsQuery : IRequest<ApiResponse<List<StudentDto>>> { }
