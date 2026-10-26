using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Features.Dashboard.Dtos;

namespace TheMathAndScienceAcademy.Application.Features.Dashboard.Query.Models;

public sealed class GetDashboardSummaryQuery : IRequest<ApiResponse<DashboardSummaryDto>>
{
}
