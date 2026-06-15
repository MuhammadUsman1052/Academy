using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Domain.Repositories;

namespace TheMathAndScienceAcademy.Api.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IUserRepository _userRepository;
    private readonly IPermissionService _permissionService;

    public PermissionAuthorizationHandler(IUserRepository userRepository, IPermissionService permissionService)
    {
        _userRepository = userRepository;
        _permissionService = permissionService;
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
        if (user is null || string.IsNullOrWhiteSpace(user.RoleId))
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
