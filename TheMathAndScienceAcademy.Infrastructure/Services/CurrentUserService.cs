using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TheMathAndScienceAcademy.Application.Abstractions;

namespace TheMathAndScienceAcademy.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirst("userId")?.Value;
    public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirst("email")?.Value;
    public string? RoleId => _httpContextAccessor.HttpContext?.User?.FindFirst("roleId")?.Value;
    public string? AcademyId => _httpContextAccessor.HttpContext?.User?.FindFirst("academyId")?.Value;
}
