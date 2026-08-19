using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using TheMathAndScienceAcademy.Application.Abstractions;
using TheMathAndScienceAcademy.Application.Common;

namespace TheMathAndScienceAcademy.Api.Authorization;

public class ApiAuthorizationResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

    public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
    {
        if (authorizeResult.Challenged)
        {
            await WriteErrorAsync(context, StatusCodes.Status401Unauthorized, ResponseMessages.Unauthorized);
            return;
        }

        if (authorizeResult.Forbidden)
        {
            var message = ResolveForbiddenMessage(authorizeResult);
            await WriteErrorAsync(context, StatusCodes.Status403Forbidden, message);
            return;
        }

        await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }

    private static string ResolveForbiddenMessage(PolicyAuthorizationResult authorizeResult)
    {
        var permissionRequirement = authorizeResult.AuthorizationFailure?.FailedRequirements
            .OfType<PermissionRequirement>()
            .FirstOrDefault();

        if (permissionRequirement is not null)
        {
            return $"You do not have permission to access '{permissionRequirement.PermissionName}'.";
        }

        return ResponseMessages.Forbidden;
    }

    private static async Task WriteErrorAsync(HttpContext context, int statusCode, string message)
    {
        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var payload = new ApiResponse<object?>(false, message, null);
        await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
}
