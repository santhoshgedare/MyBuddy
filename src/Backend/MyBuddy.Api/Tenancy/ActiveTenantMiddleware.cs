using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using MyBuddy.Application.Tenancy;

namespace MyBuddy.Api.Tenancy;

public sealed class ActiveTenantMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ActiveTenantResolver tenantResolver)
    {
        if (!context.Request.Path.StartsWithSegments("/api"))
        {
            await next(context);
            return;
        }

        if (context.User.Identity?.IsAuthenticated != true
            || !Guid.TryParse(context.User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        {
            await WriteProblemAsync(context, StatusCodes.Status401Unauthorized,
                "Authentication required", "A validated user identity is required.");
            return;
        }

        if (!Guid.TryParse(context.Request.Headers["X-Tenant-Id"], out var tenantId))
        {
            await WriteProblemAsync(context, StatusCodes.Status400BadRequest,
                "Tenant selection required", "Provide a valid X-Tenant-Id header.");
            return;
        }

        if (!await tenantResolver.TrySetActiveTenantAsync(userId, tenantId, context.RequestAborted))
        {
            await WriteProblemAsync(context, StatusCodes.Status403Forbidden,
                "Tenant access denied", "The authenticated user is not an active member of this tenant.");
            return;
        }

        await next(context);
    }

    private static Task WriteProblemAsync(
        HttpContext context,
        int status,
        string title,
        string detail)
    {
        context.Response.StatusCode = status;
        return context.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path
            },
            contentType: "application/problem+json",
            options: null,
            cancellationToken: context.RequestAborted);
    }
}
