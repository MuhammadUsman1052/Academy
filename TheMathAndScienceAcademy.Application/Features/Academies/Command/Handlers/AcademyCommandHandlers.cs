using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Common;
using TheMathAndScienceAcademy.Application.Features.Academies.Command.Models;
using TheMathAndScienceAcademy.Application.Features.Academies.Dtos;
using TheMathAndScienceAcademy.Domain.Entities;
using TheMathAndScienceAcademy.Domain.Repositories;

namespace TheMathAndScienceAcademy.Application.Features.Academies.Command.Handlers;

public class AcademyCommandHandlers : ResponseHandler,
    IRequestHandler<CreateAcademyCommand, ApiResponse<AcademyDto>>,
    IRequestHandler<UpdateAcademyCommand, ApiResponse<AcademyDto>>,
    IRequestHandler<DeleteAcademyCommand, ApiResponse<bool>>
{
    private const string AcademyAdminRoleName = "AcademyAdmin";

    private readonly IAcademyRepository _academyRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRolePermissionSyncService _rolePermissionSyncService;
    private readonly ITemporaryPasswordGenerator _passwordGenerator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _emailService;
    private readonly ILogger<AcademyCommandHandlers> _logger;
    private readonly IMapper _mapper;
    private readonly IAcademyScopeService _academyScopeService;

    public AcademyCommandHandlers(
        IAcademyRepository academyRepository,
        IRoleRepository roleRepository,
        IUserRepository userRepository,
        IRolePermissionSyncService rolePermissionSyncService,
        ITemporaryPasswordGenerator passwordGenerator,
        IPasswordHasher passwordHasher,
        IEmailService emailService,
        ILogger<AcademyCommandHandlers> logger,
        IMapper mapper,
        IAcademyScopeService academyScopeService)
    {
        _academyRepository = academyRepository;
        _roleRepository = roleRepository;
        _userRepository = userRepository;
        _rolePermissionSyncService = rolePermissionSyncService;
        _passwordGenerator = passwordGenerator;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
        _logger = logger;
        _mapper = mapper;
        _academyScopeService = academyScopeService;
    }

    public async Task<ApiResponse<AcademyDto>> Handle(CreateAcademyCommand request, CancellationToken cancellationToken)
    {
        var scope = await _academyScopeService.GetScopeAsync();
        if (!scope.IsValid || !scope.IsSuperAdmin)
            return BadRequest<AcademyDto>(ResponseMessages.Forbidden);

        var existingAdminUser = await _userRepository.GetByEmailAsync(request.AdminEmail);
        if (existingAdminUser is not null && !await IsReusableOrphanedAdminAsync(existingAdminUser))
            return BadRequest<AcademyDto>(ResponseMessages.AcademyAdminAlreadyExists);

        var academy = _mapper.Map<Academy>(request);
        var createdAcademy = await _academyRepository.CreateAsync(academy);

        if (createdAcademy is null)
        {
            return BadRequest<AcademyDto>(ResponseMessages.AcademyCreateFailed);
        }

        var adminRole = await _roleRepository.CreateAsync(new Role
        {
            Name = AcademyAdminRoleName,
            Description = $"Default academy administrator role for {createdAcademy.Name}",
            AcademyId = createdAcademy.Id
        });

        if (adminRole is null)
        {
            return BadRequest<AcademyDto>(ResponseMessages.RoleCreateFailed);
        }

        await _rolePermissionSyncService.SyncRoleAsync(adminRole.Id);

        var temporaryPassword = _passwordGenerator.Generate();
        var passwordHash = _passwordHasher.HashPassword(temporaryPassword);

        var academyAdminUser = existingAdminUser ?? new User();
        academyAdminUser.Name = request.AdminName;
        academyAdminUser.Email = request.AdminEmail.Trim().ToLowerInvariant();
        academyAdminUser.PasswordHash = passwordHash;
        academyAdminUser.RoleId = adminRole.Id;
        academyAdminUser.AcademyId = createdAcademy.Id;
        academyAdminUser.IsActive = true;
        academyAdminUser.MustChangePassword = true;
        academyAdminUser.RefreshToken = null;
        academyAdminUser.RefreshTokenExpiry = null;

        if (existingAdminUser is null)
        {
            await _userRepository.CreateAsync(academyAdminUser);
        }
        else if (!await _userRepository.UpdateAsync(academyAdminUser))
        {
            _logger.LogError("Failed to reuse orphaned academy admin user {UserId} for academy {AcademyId}",
                academyAdminUser.Id, createdAcademy.Id);
            return BadRequest<AcademyDto>(ResponseMessages.AcademyCreateFailed);
        }

        var response = _mapper.Map<AcademyDto>(createdAcademy);
        response.AdminRoleId = Guid.Parse(adminRole.Id);
        response.AdminUserId = Guid.Parse(academyAdminUser.Id);
        response.AdminRoleName = adminRole.Name;
        response.AdminName = academyAdminUser.Name;
        response.AdminEmail = academyAdminUser.Email;
        response.UserCount = 1;
        try
        {
            await _emailService.SendAcademyAdminCredentialsAsync(request.Name, request.AdminName, request.AdminEmail, temporaryPassword);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send academy admin credentials email for academy {AcademyName} to {AdminEmail}", request.Name, request.AdminEmail);
        }

        return Created(response, ResponseMessages.AcademyCreated);
    }

    private async Task<bool> IsReusableOrphanedAdminAsync(User user)
    {
        // A global user must never be reused. Only a user linked to a deleted academy
        // can be safely reclaimed for a newly created academy.
        if (string.IsNullOrWhiteSpace(user.AcademyId)
            || !Guid.TryParse(user.AcademyId, out var userAcademyId)
            || await _academyRepository.GetByIdAsync(userAcademyId) is not null)
            return false;

        if (!Guid.TryParse(user.RoleId, out var roleId))
            return true;

        var role = await _roleRepository.GetByIdAsync(roleId);
        return role is null
            || string.Equals(role.Name, AcademyAdminRoleName, StringComparison.OrdinalIgnoreCase);
    }

    public async Task<ApiResponse<AcademyDto>> Handle(UpdateAcademyCommand request, CancellationToken cancellationToken)
    {
        var scope = await _academyScopeService.GetScopeAsync();
        if (!scope.IsValid || !scope.IsSuperAdmin)
            return BadRequest<AcademyDto>(ResponseMessages.Forbidden);

        var existingAcademy = await _academyRepository.GetByIdAsync(request.Id);
        if (existingAcademy is null)
        {
            return NotFound<AcademyDto>(ResponseMessages.AcademyNotFound);
        }

        _mapper.Map(request, existingAcademy);
        var updated = await _academyRepository.UpdateAsync(existingAcademy);

        if (!updated)
        {
            return BadRequest<AcademyDto>(ResponseMessages.AcademyUpdateFailed);
        }

        return Updated(_mapper.Map<AcademyDto>(existingAcademy), ResponseMessages.AcademyUpdated);
    }

    public async Task<ApiResponse<bool>> Handle(DeleteAcademyCommand request, CancellationToken cancellationToken)
    {
        var scope = await _academyScopeService.GetScopeAsync();
        if (!scope.IsValid || !scope.IsSuperAdmin)
            return BadRequest<bool>(ResponseMessages.Forbidden);

        var deleted = await _academyRepository.DeleteAsync(request.Id);

        if (!deleted)
        {
            return NotFound<bool>(ResponseMessages.AcademyNotFound);
        }

        return Deleted<bool>(ResponseMessages.AcademyDeleted);
    }
}
