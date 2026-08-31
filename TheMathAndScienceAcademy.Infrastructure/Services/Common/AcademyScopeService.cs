using Microsoft.AspNetCore.Http;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Domain.Repositories;

namespace TheMathAndScienceAcademy.Infrastructure.Services.Common;

public sealed class AcademyScopeService : IAcademyScopeService
{
    private const string SuperAdminRoleName = "SuperAdmin";
    private const string AcademyHeaderName = "X-Academy-Id";

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICurrentUserService _currentUserService;
    private readonly IRoleRepository _roleRepository;
    private readonly IAcademyRepository _academyRepository;

    public AcademyScopeService(
        IHttpContextAccessor httpContextAccessor,
        ICurrentUserService currentUserService,
        IRoleRepository roleRepository,
        IAcademyRepository academyRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _currentUserService = currentUserService;
        _roleRepository = roleRepository;
        _academyRepository = academyRepository;
    }

    public async Task<AcademyScope> GetScopeAsync()
    {
        if (!Guid.TryParse(_currentUserService.RoleId, out var roleId))
            return new AcademyScope(false, null, false, false);

        var role = await _roleRepository.GetByIdAsync(roleId);
        if (role is null)
            return new AcademyScope(false, null, false, false);

        var isSuperAdmin = string.Equals(role.Name, SuperAdminRoleName, StringComparison.OrdinalIgnoreCase);
        var selectedValue = _httpContextAccessor.HttpContext?.Request.Headers[AcademyHeaderName].FirstOrDefault();
        if (!isSuperAdmin)
        {
            var academyId = _currentUserService.AcademyId ?? role.AcademyId;
            if (!string.IsNullOrWhiteSpace(selectedValue)
                && !string.Equals(selectedValue, academyId, StringComparison.OrdinalIgnoreCase))
                return new AcademyScope(false, academyId, true, false, true);

            var valid = !string.IsNullOrWhiteSpace(academyId) && Guid.TryParse(academyId, out var parsedAcademyId)
                && await _academyRepository.GetByIdAsync(parsedAcademyId) is not null;
            return new AcademyScope(false, academyId, !string.IsNullOrWhiteSpace(academyId), valid);
        }

        if (string.IsNullOrWhiteSpace(selectedValue))
            return new AcademyScope(true, null, false, true);

        if (!Guid.TryParse(selectedValue, out var selectedAcademyId))
            return new AcademyScope(true, selectedValue, true, false);

        var academy = await _academyRepository.GetByIdAsync(selectedAcademyId);
        return new AcademyScope(true, selectedAcademyId.ToString(), true, academy is not null);
    }
}
