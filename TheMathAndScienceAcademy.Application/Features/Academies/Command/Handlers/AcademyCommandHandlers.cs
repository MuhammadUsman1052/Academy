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
    private readonly ITemporaryPasswordGenerator _passwordGenerator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _emailService;
    private readonly ILogger<AcademyCommandHandlers> _logger;
    private readonly IMapper _mapper;

    public AcademyCommandHandlers(
        IAcademyRepository academyRepository,
        IRoleRepository roleRepository,
        IUserRepository userRepository,
        ITemporaryPasswordGenerator passwordGenerator,
        IPasswordHasher passwordHasher,
        IEmailService emailService,
        ILogger<AcademyCommandHandlers> logger,
        IMapper mapper)
    {
        _academyRepository = academyRepository;
        _roleRepository = roleRepository;
        _userRepository = userRepository;
        _passwordGenerator = passwordGenerator;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<ApiResponse<AcademyDto>> Handle(CreateAcademyCommand request, CancellationToken cancellationToken)
    {
        var academy = _mapper.Map<Academy>(request);
        var createdAcademy = await _academyRepository.CreateAsync(academy);

        if (createdAcademy is null)
        {
            return BadRequest<AcademyDto>(ResponseMessages.AcademyCreateFailed);
        }

        var academyAdminRole = await _roleRepository.GetByNameAsync(AcademyAdminRoleName);
        if (academyAdminRole is null)
        {
            academyAdminRole = await _roleRepository.CreateAsync(new Role
            {
                Name = AcademyAdminRoleName,
                Description = "Default academy administrator role"
            });
        }

        if (academyAdminRole is null)
        {
            return BadRequest<AcademyDto>(ResponseMessages.AcademyAdminRoleCreateFailed);
        }

        var temporaryPassword = _passwordGenerator.Generate();
        var passwordHash = _passwordHasher.HashPassword(temporaryPassword);

        var existingAdminUser = await _userRepository.GetByEmailAsync(request.AdminEmail);
        if (existingAdminUser is not null)
        {
            return BadRequest<AcademyDto>(ResponseMessages.AcademyAdminAlreadyExists);
        }

        var academyAdminUser = new User
        {
            Name = request.AdminName,
            Email = request.AdminEmail,
            PasswordHash = passwordHash,
            RoleId = academyAdminRole.Id,
            IsActive = true,
            MustChangePassword = true
        };

        await _userRepository.CreateAsync(academyAdminUser);
        try
        {
            await _emailService.SendAcademyAdminCredentialsAsync(request.Name, request.AdminName, request.AdminEmail, temporaryPassword);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send academy admin credentials email for academy {AcademyName} to {AdminEmail}", request.Name, request.AdminEmail);
        }

        return Created(_mapper.Map<AcademyDto>(createdAcademy), ResponseMessages.AcademyCreated);
    }

    public async Task<ApiResponse<AcademyDto>> Handle(UpdateAcademyCommand request, CancellationToken cancellationToken)
    {
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
        var deleted = await _academyRepository.DeleteAsync(request.Id);

        if (!deleted)
        {
            return NotFound<bool>(ResponseMessages.AcademyNotFound);
        }

        return Deleted<bool>(ResponseMessages.AcademyDeleted);
    }
}
