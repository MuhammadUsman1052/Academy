using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Domain.Repositories;

namespace TheMathAndScienceAcademy.Api.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IUserRepository _userRepository;
    private readonly IPermissionService _permissionService;
    private readonly IAcademyRepository _academyRepository;

    public PermissionAuthorizationHandler(IUserRepository userRepository, IPermissionService permissionService, IAcademyRepository academyRepository)
    {
        _userRepository = userRepository;
        _permissionService = permissionService;
        _academyRepository = academyRepository;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var userId = context.User.FindFirst("userId")?.Value;
        if (string.IsNullOrWhiteSpace(userId))
        {
            context.Fail();
            return;
        }

        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null || !user.IsActive || string.IsNullOrWhiteSpace(user.RoleId))
        {
            context.Fail();
            return;
        }

        if (!string.IsNullOrWhiteSpace(user.AcademyId)
            && (!Guid.TryParse(user.AcademyId, out var academyId)
                || await _academyRepository.GetByIdAsync(academyId) is null))
        {
            context.Fail();
            return;
        }

        var hasPermission = await _permissionService.HasPermissionAsync(user.RoleId, requirement.PermissionName);
        if (!hasPermission)
        {
            context.Fail();
            return;
        }

        context.Succeed(requirement);
    }
}
