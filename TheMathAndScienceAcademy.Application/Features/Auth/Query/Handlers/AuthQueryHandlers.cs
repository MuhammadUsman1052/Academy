using AutoMapper;
using MediatR;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Common;
using TheMathAndScienceAcademy.Application.Features.Auth.Dtos;
using TheMathAndScienceAcademy.Application.Features.Auth.Query.Models;
using TheMathAndScienceAcademy.Domain.Repositories;

namespace TheMathAndScienceAcademy.Application.Features.Auth.Query.Handlers;

public class AuthQueryHandlers : ResponseHandler,
    IRequestHandler<GetCurrentUserQuery, ApiResponse<CurrentUserDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public AuthQueryHandlers(
        ICurrentUserService currentUserService,
        IUserRepository userRepository,
        IMapper mapper)
    {
        _currentUserService = currentUserService;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<CurrentUserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_currentUserService.UserId))
            return BadRequest<CurrentUserDto>(ResponseMessages.Unauthorized);

        var user = await _userRepository.GetByIdAsync(_currentUserService.UserId);
        if (user is null)
            return NotFound<CurrentUserDto>(ResponseMessages.UserNotFound);

        var dto = _mapper.Map<CurrentUserDto>(user);
        dto.RoleId = _currentUserService.RoleId ?? dto.RoleId;
        dto.AcademyId = _currentUserService.AcademyId ?? dto.AcademyId;

        return Success(dto);
    }
}
