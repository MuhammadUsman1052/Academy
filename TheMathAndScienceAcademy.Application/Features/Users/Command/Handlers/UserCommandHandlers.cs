using AutoMapper;
using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Common;
using TheMathAndScienceAcademy.Application.Features.Users.Command.Models;
using TheMathAndScienceAcademy.Application.Features.Users.Dtos;
using TheMathAndScienceAcademy.Domain.Entities;
using TheMathAndScienceAcademy.Domain.Repositories;

namespace TheMathAndScienceAcademy.Application.Features.Users.Command.Handlers;

public class UserCommandHandlers : ResponseHandler,
    IRequestHandler<CreateUserCommand, ApiResponse<UserDto>>,
    IRequestHandler<UpdateUserCommand, ApiResponse<UserDto>>,
    IRequestHandler<DeleteUserCommand, ApiResponse<bool>>
{
    private const string SuperAdminRoleName = "SuperAdmin";

    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IAcademyRepository _academyRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IMapper _mapper;

    public UserCommandHandlers(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IAcademyRepository academyRepository,
        ICurrentUserService currentUserService,
        IPasswordHasher passwordHasher,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _academyRepository = academyRepository;
        _currentUserService = currentUserService;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
    }

    public async Task<ApiResponse<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var access = await GetAccessContextAsync();
        if (access is null)
        {
            return BadRequest<UserDto>(ResponseMessages.Unauthorized);
        }

        var role = await _roleRepository.GetByIdAsync(request.RoleId);
        if (role is null)
        {
            return NotFound<UserDto>(ResponseMessages.RoleNotFound);
        }

        var targetAcademyId = request.AcademyId?.ToString();
        if (!access.IsSuperAdmin)
        {
            if (string.IsNullOrWhiteSpace(access.AcademyId))
            {
                return BadRequest<UserDto>(ResponseMessages.Forbidden);
            }

            if (role.AcademyId is not null && !string.Equals(role.AcademyId, access.AcademyId, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest<UserDto>(ResponseMessages.Forbidden);
            }

            targetAcademyId = access.AcademyId;
        }
        else if (role.AcademyId is not null)
        {
            if (string.IsNullOrWhiteSpace(targetAcademyId))
            {
                targetAcademyId = role.AcademyId;
            }
            else if (!string.Equals(role.AcademyId, targetAcademyId, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest<UserDto>(ResponseMessages.Forbidden);
            }
        }

        if (targetAcademyId is not null)
        {
            var academy = await _academyRepository.GetByIdAsync(Guid.Parse(targetAcademyId));
            if (academy is null)
            {
                return NotFound<UserDto>(ResponseMessages.AcademyNotFound);
            }
        }

        var existing = await _userRepository.GetByEmailAsync(request.Email);
        if (existing is not null)
        {
            return BadRequest<UserDto>(ResponseMessages.UserAlreadyExists);
        }

        var user = new User
        {
            Name = request.Name.Trim(),
            Email = request.Email.Trim(),
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            RoleId = role.Id,
            AcademyId = targetAcademyId,
            IsActive = request.IsActive,
            MustChangePassword = request.MustChangePassword
        };

        await _userRepository.CreateAsync(user);
        return Created(_mapper.Map<UserDto>(user), ResponseMessages.UserCreated);
    }

    public async Task<ApiResponse<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var access = await GetAccessContextAsync();
        if (access is null)
        {
            return BadRequest<UserDto>(ResponseMessages.Unauthorized);
        }

        var existing = await _userRepository.GetByIdAsync(request.Id.ToString());
        if (existing is null)
        {
            return NotFound<UserDto>(ResponseMessages.UserNotFound);
        }

        if (!access.IsSuperAdmin && !string.Equals(existing.AcademyId, access.AcademyId, StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest<UserDto>(ResponseMessages.Forbidden);
        }

        var duplicateEmail = await _userRepository.GetByEmailAsync(request.Email);
        if (duplicateEmail is not null && duplicateEmail.Id != existing.Id)
        {
            return BadRequest<UserDto>(ResponseMessages.UserAlreadyExists);
        }

        var role = await _roleRepository.GetByIdAsync(request.RoleId);
        if (role is null)
        {
            return NotFound<UserDto>(ResponseMessages.RoleNotFound);
        }

        var targetAcademyId = request.AcademyId?.ToString() ?? existing.AcademyId;
        if (!access.IsSuperAdmin)
        {
            if (string.IsNullOrWhiteSpace(access.AcademyId))
            {
                return BadRequest<UserDto>(ResponseMessages.Forbidden);
            }

            if (role.AcademyId is not null && !string.Equals(role.AcademyId, access.AcademyId, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest<UserDto>(ResponseMessages.Forbidden);
            }

            targetAcademyId = access.AcademyId;
        }
        else if (role.AcademyId is not null)
        {
            if (string.IsNullOrWhiteSpace(targetAcademyId))
            {
                targetAcademyId = role.AcademyId;
            }
            else if (!string.Equals(role.AcademyId, targetAcademyId, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest<UserDto>(ResponseMessages.Forbidden);
            }
        }

        if (targetAcademyId is not null)
        {
            var academy = await _academyRepository.GetByIdAsync(Guid.Parse(targetAcademyId));
            if (academy is null)
            {
                return NotFound<UserDto>(ResponseMessages.AcademyNotFound);
            }
        }

        existing.Name = request.Name.Trim();
        existing.Email = request.Email.Trim();
        existing.RoleId = role.Id;
        existing.AcademyId = targetAcademyId;
        existing.IsActive = request.IsActive;
        existing.MustChangePassword = request.MustChangePassword;
        existing.UpdatedAt = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            existing.PasswordHash = _passwordHasher.HashPassword(request.Password);
        }

        var updated = await _userRepository.UpdateAsync(existing);
        if (!updated)
        {
            return BadRequest<UserDto>(ResponseMessages.UserUpdateFailed);
        }

        return Updated(_mapper.Map<UserDto>(existing), ResponseMessages.UserUpdated);
    }

    public async Task<ApiResponse<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var access = await GetAccessContextAsync();
        if (access is null)
        {
            return BadRequest<bool>(ResponseMessages.Unauthorized);
        }

        var existing = await _userRepository.GetByIdAsync(request.Id.ToString());
        if (existing is null)
        {
            return NotFound<bool>(ResponseMessages.UserNotFound);
        }

        if (!access.IsSuperAdmin && !string.Equals(existing.AcademyId, access.AcademyId, StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest<bool>(ResponseMessages.Forbidden);
        }

        var deleted = await _userRepository.DeleteAsync(request.Id.ToString());
        if (!deleted)
        {
            return NotFound<bool>(ResponseMessages.UserNotFound);
        }

        return Deleted<bool>(ResponseMessages.UserDeleted);
    }

    private async Task<UserAccessContext?> GetAccessContextAsync()
    {
        if (!Guid.TryParse(_currentUserService.RoleId, out var roleId))
        {
            return null;
        }

        var currentRole = await _roleRepository.GetByIdAsync(roleId);
        if (currentRole is null)
        {
            return null;
        }

        var isSuperAdmin = string.Equals(currentRole.Name, SuperAdminRoleName, StringComparison.OrdinalIgnoreCase);
        var academyId = string.IsNullOrWhiteSpace(_currentUserService.AcademyId)
            ? currentRole.AcademyId
            : _currentUserService.AcademyId;

        return new UserAccessContext(isSuperAdmin, academyId);
    }

    private sealed record UserAccessContext(bool IsSuperAdmin, string? AcademyId);
}
