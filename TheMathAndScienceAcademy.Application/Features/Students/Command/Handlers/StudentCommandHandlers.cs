using AutoMapper;
using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Common;
using TheMathAndScienceAcademy.Application.Features.Students.Command.Models;
using TheMathAndScienceAcademy.Application.Features.Students.Dtos;
using TheMathAndScienceAcademy.Domain.Entities;
using TheMathAndScienceAcademy.Domain.Repositories;

namespace TheMathAndScienceAcademy.Application.Features.Students.Command.Handlers;

public class StudentCommandHandlers : ResponseHandler,
    IRequestHandler<CreateStudentCommand, ApiResponse<StudentDto>>,
    IRequestHandler<CreateStudentAccountCommand, ApiResponse<StudentDto>>,
    IRequestHandler<UpdateStudentCommand, ApiResponse<StudentDto>>,
    IRequestHandler<DeleteStudentCommand, ApiResponse<bool>>,
    IRequestHandler<ResendStudentCredentialsCommand, ApiResponse<bool>>
{
    private const string StudentRoleName = "Student";
    private const string SelfViewPermission = "student.self-view";
    private readonly IStudentRepository _students;
    private readonly IAcademyRepository _academies;
    private readonly IUserRepository _users;
    private readonly IRoleRepository _roles;
    private readonly IPermissionRepository _permissions;
    private readonly IRolePermissionRepository _rolePermissions;
    private readonly IRolePermissionSyncService _sync;
    private readonly IAcademyScopeService _scope;
    private readonly IPasswordHasher _hasher;
    private readonly ITemporaryPasswordGenerator _passwords;
    private readonly IEmailService _email;
    private readonly ITransactionService _transactions;
    private readonly IMapper _mapper;

    public StudentCommandHandlers(IStudentRepository students, IAcademyRepository academies, IUserRepository users,
        IRoleRepository roles, IPermissionRepository permissions, IRolePermissionRepository rolePermissions,
        IRolePermissionSyncService sync, IAcademyScopeService scope, IPasswordHasher hasher,
        ITemporaryPasswordGenerator passwords, IEmailService email, ITransactionService transactions, IMapper mapper)
    {
        _students = students; _academies = academies; _users = users; _roles = roles; _permissions = permissions;
        _rolePermissions = rolePermissions; _sync = sync; _scope = scope; _hasher = hasher; _passwords = passwords;
        _email = email; _transactions = transactions; _mapper = mapper;
    }

    public async Task<ApiResponse<StudentDto>> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
    {
        var academyId = await ResolveAcademyIdAsync(request.AcademyId);
        if (academyId is null) return BadRequest<StudentDto>(ResponseMessages.AcademySelectionRequired);
        if (await _academies.GetByIdAsync(Guid.Parse(academyId)) is null)
            return NotFound<StudentDto>(ResponseMessages.AcademyNotFound);
        if (await _students.GetByEnrollmentNumberAsync(academyId, request.EnrollmentNumber.Trim()) is not null)
            return BadRequest<StudentDto>(ResponseMessages.StudentAlreadyExists);

        var student = new Student
        {
            EnrollmentNumber = request.EnrollmentNumber.Trim(),
            FullName = request.FullName.Trim(),
            Email = NormalizeEmail(request.Email),
            PhoneNumber = Normalize(request.PhoneNumber),
            DateOfBirth = ToUtcDate(request.DateOfBirth),
            Gender = Normalize(request.Gender),
            Address = Normalize(request.Address),
            AdmissionDate = ToUtcDate(request.AdmissionDate)!.Value,
            Status = NormalizeStatus(request.Status),
            AcademyId = academyId,
            UserId = null
        };

        var created = await _students.CreateAsync(student);
        return created is null
            ? BadRequest<StudentDto>(ResponseMessages.StudentCreateFailed)
            : Created(await ToDtoAsync(created), ResponseMessages.StudentProfileCreated);
    }

    public async Task<ApiResponse<StudentDto>> Handle(CreateStudentAccountCommand request, CancellationToken cancellationToken)
    {
        var scope = await _scope.GetScopeAsync();
        var student = await _students.GetByIdAsync(request.StudentId.ToString());
        if (student is null) return NotFound<StudentDto>(ResponseMessages.StudentNotFound);
        if (!CanAccess(scope, student.AcademyId)) return BadRequest<StudentDto>(ResponseMessages.Forbidden);
        if (!string.IsNullOrWhiteSpace(student.UserId)) return BadRequest<StudentDto>(ResponseMessages.StudentAccountAlreadyExists);
        if (string.IsNullOrWhiteSpace(student.Email)) return BadRequest<StudentDto>(ResponseMessages.StudentEmailRequired);

        var email = student.Email.Trim().ToLowerInvariant();
        if (await _users.GetByEmailAsync(email) is not null)
            return BadRequest<StudentDto>(ResponseMessages.StudentUserAlreadyExists);
        var academy = await _academies.GetByIdAsync(Guid.Parse(student.AcademyId));
        if (academy is null) return NotFound<StudentDto>(ResponseMessages.AcademyNotFound);

        var temporaryPassword = _passwords.Generate();
        ProvisionedStudent provisioned;
        try
        {
            provisioned = await _transactions.ExecuteAsync(async () =>
            {
                var role = await _roles.GetByNameAsync(StudentRoleName, student.AcademyId);
                if (role is null)
                {
                    role = await _roles.CreateAsync(new Role { Name = StudentRoleName, Description = "Student login role for this academy.", AcademyId = student.AcademyId });
                    if (role is null) throw new InvalidOperationException(ResponseMessages.StudentRoleCreateFailed);
                }

                await _sync.SyncRoleAsync(role.Id);
                var permission = await _permissions.GetByNameAsync(SelfViewPermission);
                if (permission is null) throw new InvalidOperationException(ResponseMessages.StudentRoleCreateFailed);
                await _rolePermissions.AssignPermissionAsync(role.Id, permission.Id);

                var user = new User
                {
                    Name = student.FullName,
                    Email = email,
                    PasswordHash = _hasher.HashPassword(temporaryPassword),
                    RoleId = role.Id,
                    AcademyId = student.AcademyId,
                    IsActive = false,
                    MustChangePassword = true
                };
                await _users.CreateAsync(user);
                student.Email = email;
                student.UserId = user.Id;
                student.UpdatedAt = DateTime.UtcNow;
                if (!await _students.UpdateAsync(student))
                    throw new InvalidOperationException(ResponseMessages.StudentProfileLinkFailed);
                return new ProvisionedStudent(student, user);
            }, cancellationToken);
        }
        catch (InvalidOperationException ex) { return BadRequest<StudentDto>(ex.Message); }
        catch { return BadRequest<StudentDto>(ResponseMessages.StudentAccountCreateFailed); }

        try
        {
            await _email.SendStudentCredentialsAsync(academy.Name, provisioned.Student.FullName, provisioned.User.Email, temporaryPassword);
            provisioned.User.IsActive = true;
            provisioned.User.CredentialsSentAt = DateTime.UtcNow;
            await _users.UpdateAsync(provisioned.User);
            return Created(await ToDtoAsync(provisioned.Student), ResponseMessages.StudentAccountCreated);
        }
        catch
        {
            provisioned.User.IsActive = false;
            provisioned.User.CredentialsSentAt = null;
            await _users.UpdateAsync(provisioned.User);
            return Created(await ToDtoAsync(provisioned.Student), ResponseMessages.StudentCredentialsPending);
        }
    }

    public async Task<ApiResponse<StudentDto>> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
    {
        var scope = await _scope.GetScopeAsync();
        var student = await _students.GetByIdAsync(request.Id.ToString());
        if (student is null) return NotFound<StudentDto>(ResponseMessages.StudentNotFound);
        if (!CanAccess(scope, student.AcademyId)) return BadRequest<StudentDto>(ResponseMessages.Forbidden);
        var duplicate = await _students.GetByEnrollmentNumberAsync(student.AcademyId, request.EnrollmentNumber.Trim());
        if (duplicate is not null && duplicate.Id != student.Id) return BadRequest<StudentDto>(ResponseMessages.StudentAlreadyExists);

        var email = NormalizeEmail(request.Email);
        if (email is null && student.UserId is not null) return BadRequest<StudentDto>(ResponseMessages.StudentEmailRequired);
        if (email is not null)
        {
            var existing = await _users.GetByEmailAsync(email);
            if (existing is not null && existing.Id != student.UserId) return BadRequest<StudentDto>(ResponseMessages.StudentUserAlreadyExists);
        }

        var user = student.UserId is null ? null : await _users.GetByIdAsync(student.UserId);
        student.EnrollmentNumber = request.EnrollmentNumber.Trim();
        student.FullName = request.FullName.Trim();
        student.Email = email;
        student.PhoneNumber = Normalize(request.PhoneNumber);
        student.DateOfBirth = ToUtcDate(request.DateOfBirth);
        student.Gender = Normalize(request.Gender);
        student.Address = Normalize(request.Address);
        student.AdmissionDate = ToUtcDate(request.AdmissionDate)!.Value;
        student.Status = NormalizeStatus(request.Status);
        student.UpdatedAt = DateTime.UtcNow;
        if (user is not null)
        {
            user.Name = student.FullName;
            if (email is not null) user.Email = email;
            await _users.UpdateAsync(user);
        }
        if (!await _students.UpdateAsync(student)) return BadRequest<StudentDto>(ResponseMessages.StudentUpdateFailed);
        return Updated(await ToDtoAsync(student), ResponseMessages.StudentUpdated);
    }

    public async Task<ApiResponse<bool>> Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
    {
        var scope = await _scope.GetScopeAsync();
        var student = await _students.GetByIdAsync(request.Id.ToString());
        if (student is null) return NotFound<bool>(ResponseMessages.StudentNotFound);
        if (!CanAccess(scope, student.AcademyId)) return BadRequest<bool>(ResponseMessages.Forbidden);
        var deleted = await _transactions.ExecuteAsync(async () =>
        {
            if (!await _students.DeleteAsync(student.Id)) return false;
            if (student.UserId is not null) await _users.DeleteAsync(student.UserId);
            return true;
        }, cancellationToken);
        if (!deleted) return NotFound<bool>(ResponseMessages.StudentNotFound);
        return Deleted<bool>(ResponseMessages.StudentDeleted);
    }

    public async Task<ApiResponse<bool>> Handle(ResendStudentCredentialsCommand request, CancellationToken cancellationToken)
    {
        var scope = await _scope.GetScopeAsync();
        var student = await _students.GetByIdAsync(request.StudentId.ToString());
        if (student is null) return NotFound<bool>(ResponseMessages.StudentNotFound);
        if (!CanAccess(scope, student.AcademyId)) return BadRequest<bool>(ResponseMessages.Forbidden);
        if (student.UserId is null) return BadRequest<bool>(ResponseMessages.StudentNotLinked);
        var user = await _users.GetByIdAsync(student.UserId);
        var academy = await _academies.GetByIdAsync(Guid.Parse(student.AcademyId));
        if (user is null) return BadRequest<bool>(ResponseMessages.StudentNotLinked);
        if (academy is null) return NotFound<bool>(ResponseMessages.AcademyNotFound);

        var password = _passwords.Generate();
        user.PasswordHash = _hasher.HashPassword(password);
        user.MustChangePassword = true;
        user.IsActive = false;
        user.CredentialsSentAt = null;
        await _users.UpdateAsync(user);
        try
        {
            await _email.SendStudentCredentialsAsync(academy.Name, student.FullName, user.Email, password);
            user.IsActive = true;
            user.CredentialsSentAt = DateTime.UtcNow;
            await _users.UpdateAsync(user);
            return Success(true, ResponseMessages.StudentCredentialsResent);
        }
        catch { return BadRequest<bool>(ResponseMessages.StudentCredentialsEmailFailed); }
    }

    private async Task<string?> ResolveAcademyIdAsync(Guid? requested)
    {
        var scope = await _scope.GetScopeAsync();
        if (!scope.IsValid || string.IsNullOrWhiteSpace(scope.AcademyId)) return null;
        if (requested.HasValue && !requested.Value.ToString().Equals(scope.AcademyId, StringComparison.OrdinalIgnoreCase)) return null;
        return scope.AcademyId;
    }

    private static bool CanAccess(AcademyScope scope, string academyId) => scope.IsValid && string.Equals(scope.AcademyId, academyId, StringComparison.OrdinalIgnoreCase);

    private async Task<StudentDto> ToDtoAsync(Student student)
    {
        var dto = _mapper.Map<StudentDto>(student);
        dto.AcademyName = (await _academies.GetByIdAsync(Guid.Parse(student.AcademyId)))?.Name;
        var user = student.UserId is null ? null : await _users.GetByIdAsync(student.UserId);
        dto.UserName = user?.Name;
        dto.LoginEnabled = user?.IsActive == true;
        dto.MustChangePassword = user?.MustChangePassword == true;
        dto.CredentialsSentAt = user?.CredentialsSentAt;
        dto.CredentialsPending = user is not null && user.CredentialsSentAt is null;
        return dto;
    }

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static string? NormalizeEmail(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToLowerInvariant();
    private static string NormalizeStatus(string value) => string.Equals(value, "Inactive", StringComparison.OrdinalIgnoreCase) ? "Inactive" : "Active";
    private static DateTime? ToUtcDate(DateTime? value) => value.HasValue ? DateTime.SpecifyKind(value.Value.Date, DateTimeKind.Utc) : null;
    private sealed record ProvisionedStudent(Student Student, User User);
}
