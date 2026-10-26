using Microsoft.AspNetCore.Mvc.Controllers;
using TheMathAndScienceAcademy.Domain.Entities;

namespace TheMathAndScienceAcademy.Api.Middleware;

public sealed class AuditLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuditLoggingMiddleware> _logger;

    public AuditLoggingMiddleware(RequestDelegate next, ILogger<AuditLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext, AppDbContext dbContext)
    {
        var isAuditable = httpContext.Request.Path.StartsWithSegments("/api")
            && !HttpMethods.IsGet(httpContext.Request.Method)
            && !httpContext.Request.Path.StartsWithSegments("/api/auth/refresh-token");

        try
        {
            await _next(httpContext);
        }
        finally
        {
            if (isAuditable)
            {
                try
                {
                    var endpoint = httpContext.GetEndpoint()?.Metadata.GetMetadata<ControllerActionDescriptor>();
                    var entity = endpoint?.ControllerName ?? "Request";
                    var action = endpoint?.ActionName ?? httpContext.Request.Method;
                    var actor = httpContext.User.FindFirst("sub")?.Value
                        ?? httpContext.User.FindFirst("userId")?.Value;
                    var academyId = httpContext.User.FindFirst("academyId")?.Value;
                    var entityId = httpContext.Request.RouteValues.TryGetValue("id", out var id) ? id?.ToString() : null;

                    dbContext.Set<AuditLog>().Add(new AuditLog
                    {
                        ActorUserId = actor,
                        Action = $"{httpContext.Request.Method} {action}",
                        Entity = entity,
                        EntityId = entityId,
                        AcademyId = academyId,
                        Result = httpContext.Response.StatusCode is >= 200 and < 400 ? "Success" : "Failed",
                        StatusCode = httpContext.Response.StatusCode
                    });
                    await dbContext.SaveChangesAsync();
                }
                catch (Exception exception)
                {
                    _logger.LogWarning(exception, "Audit log could not be saved for {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);
                }
            }
        }
    }
}
