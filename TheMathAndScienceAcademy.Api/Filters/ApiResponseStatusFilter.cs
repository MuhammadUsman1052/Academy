using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Common;

namespace TheMathAndScienceAcademy.Api.Filters;

public sealed class ApiResponseStatusFilter : IResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Result is not ObjectResult { Value: not null } result)
            return;

        var response = result.Value;
        var success = response.GetType().GetProperty(nameof(ApiResponse<object>.Success))?.GetValue(response) as bool?;
        if (success is not false)
            return;

        var message = response.GetType().GetProperty(nameof(ApiResponse<object>.Message))?.GetValue(response)?.ToString();
        result.StatusCode = message switch
        {
            ResponseMessages.Unauthorized => StatusCodes.Status401Unauthorized,
            ResponseMessages.Forbidden => StatusCodes.Status403Forbidden,
            ResponseMessages.UserNotFound or ResponseMessages.StudentNotFound or ResponseMessages.ClassNotFound
                or ResponseMessages.EnrollmentNotFound or ResponseMessages.RoleNotFound or ResponseMessages.PermissionNotFound
                or ResponseMessages.AcademyNotFound => StatusCodes.Status404NotFound,
            ResponseMessages.UserAlreadyExists or ResponseMessages.RoleAlreadyExists or ResponseMessages.PermissionAlreadyExists
                or ResponseMessages.AcademyAlreadyExists => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status400BadRequest
        };
    }

    public void OnResultExecuted(ResultExecutedContext context)
    {
    }
}
