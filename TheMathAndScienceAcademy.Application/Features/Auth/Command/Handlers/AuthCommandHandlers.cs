using AutoMapper;
using BCryptNet = BCrypt.Net.BCrypt;
using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Common;
using TheMathAndScienceAcademy.Application.Features.Auth.Command.Models;
using TheMathAndScienceAcademy.Application.Features.Auth.Dtos;
using TheMathAndScienceAcademy.Domain.Repositories;

namespace TheMathAndScienceAcademy.Application.Features.Auth.Command.Handlers;

public class AuthCommandHandlers : ResponseHandler,
    IRequestHandler<LoginCommand, ApiResponse<LoginResponseDto>>,
    IRequestHandler<ChangePasswordCommand, ApiResponse<bool>>,
    IRequestHandler<ForgotPasswordCommand, ApiResponse<bool>>,
    IRequestHandler<ResetPasswordCommand, ApiResponse<bool>>,
    IRequestHandler<RefreshTokenCommand, ApiResponse<LoginResponseDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;

    public AuthCommandHandlers(
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService,
        ICurrentUserService currentUserService,
        IEmailService emailService,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _currentUserService = currentUserService;
        _emailService = emailService;
        _mapper = mapper;
    }

    public async Task<ApiResponse<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user is null)
            return BadRequest<LoginResponseDto>(ResponseMessages.InvalidCredentials);

        if (!user.IsActive)
            return BadRequest<LoginResponseDto>(ResponseMessages.UserInactive);

        var normalizedPassword = request.Password?.Trim() ?? string.Empty;
        var isValidPassword = BCryptNet.Verify(normalizedPassword, user.PasswordHash);
        if (!isValidPassword)
        {
            var decodedPassword = Uri.UnescapeDataString(normalizedPassword);
            if (!string.Equals(decodedPassword, normalizedPassword, StringComparison.Ordinal))
            {
                isValidPassword = BCryptNet.Verify(decodedPassword, user.PasswordHash);
            }
        }

        if (!isValidPassword)
            return BadRequest<LoginResponseDto>(ResponseMessages.InvalidCredentials);

        var token = _jwtTokenService.GenerateToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await _userRepository.UpdateAsync(user);

        var response = _mapper.Map<LoginResponseDto>(user);
        response.Token = token;
        response.RefreshToken = refreshToken;

        if (user.MustChangePassword)
            return Success(response, ResponseMessages.PasswordChangeRequired);

        return Success(response, ResponseMessages.LoginSuccessful);
    }

    public async Task<ApiResponse<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_currentUserService.UserId))
            return BadRequest<bool>(ResponseMessages.Unauthorized);

        var user = await _userRepository.GetByIdAsync(_currentUserService.UserId);
        if (user is null)
            return NotFound<bool>(ResponseMessages.UserNotFound);

        var normalizedCurrentPassword = request.CurrentPassword?.Trim() ?? string.Empty;
        var isValidPassword = BCryptNet.Verify(normalizedCurrentPassword, user.PasswordHash);
        if (!isValidPassword)
        {
            var decodedCurrentPassword = Uri.UnescapeDataString(normalizedCurrentPassword);
            if (!string.Equals(decodedCurrentPassword, normalizedCurrentPassword, StringComparison.Ordinal))
            {
                isValidPassword = BCryptNet.Verify(decodedCurrentPassword, user.PasswordHash);
            }
        }

        if (!isValidPassword)
            return BadRequest<bool>(ResponseMessages.CurrentPasswordIncorrect);

        user.PasswordHash = BCryptNet.HashPassword(request.NewPassword);
        user.MustChangePassword = false;
        user.ResetPasswordToken = null;
        user.ResetPasswordTokenExpiry = null;

        var updated = await _userRepository.UpdateAsync(user);
        if (!updated)
            return BadRequest<bool>(ResponseMessages.PasswordChangeFailed);

        return Success(true, ResponseMessages.PasswordChangedSuccessfully);
    }

    public async Task<ApiResponse<bool>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
      {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user is null)
            return Success(true, ResponseMessages.PasswordResetRequestProcessed);

        user.ResetPasswordToken = Guid.NewGuid().ToString("N");
        user.ResetPasswordTokenExpiry = DateTime.UtcNow.AddMinutes(30);

        var updated = await _userRepository.UpdateAsync(user);
        if (!updated)
            return BadRequest<bool>(ResponseMessages.PasswordResetFailed);

        var subject = "Password Reset Request";
        var body = $"Dear {user.Name},\n\nUse this token to reset your password:\n{user.ResetPasswordToken}\n\nThis token expires in 30 minutes.";
        await _emailService.SendAsync(user.Email, subject, body);
        return Success(true, ResponseMessages.PasswordResetRequestProcessed);
    }

    public async Task<ApiResponse<bool>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByResetPasswordTokenAsync(request.Token);
        if (user is null || user.ResetPasswordTokenExpiry is null || user.ResetPasswordTokenExpiry < DateTime.UtcNow)
            return BadRequest<bool>(ResponseMessages.InvalidOrExpiredResetToken);

        user.PasswordHash = BCryptNet.HashPassword(request.NewPassword);
        user.MustChangePassword = false;
        user.ResetPasswordToken = null;
        user.ResetPasswordTokenExpiry = null;

        var updated = await _userRepository.UpdateAsync(user);
        if (!updated)
            return BadRequest<bool>(ResponseMessages.PasswordResetFailed);

        return Success(true, ResponseMessages.PasswordResetSuccessful);
    }

    public async Task<ApiResponse<LoginResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken);
        if (user is null || user.RefreshTokenExpiry is null || user.RefreshTokenExpiry < DateTime.UtcNow)
            return BadRequest<LoginResponseDto>(ResponseMessages.InvalidOrExpiredRefreshToken);

        var newAccessToken = _jwtTokenService.GenerateToken(user);
        var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        var updated = await _userRepository.UpdateAsync(user);
        if (!updated)
            return BadRequest<LoginResponseDto>(ResponseMessages.RefreshTokenFailed);

        var response = _mapper.Map<LoginResponseDto>(user);
        response.Token = newAccessToken;
        response.RefreshToken = newRefreshToken;

        return Success(response, ResponseMessages.RefreshTokenSuccessful);
    }
}
