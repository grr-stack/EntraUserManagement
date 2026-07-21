using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared.Authentication.Extensions;

namespace Shared.Authentication.Middleware;

/// <summary>
/// Logs the authenticated caller context for each request.
/// </summary>
public sealed class UserContextLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<UserContextLoggingMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserContextLoggingMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next delegate.</param>
    /// <param name="logger">The logger.</param>
    public UserContextLoggingMiddleware(RequestDelegate next, ILogger<UserContextLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Executes the middleware.
    /// </summary>
    /// <param name="context">The current request context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            _logger.LogInformation(
                "Authenticated request {Method} {Path} by {Name}. ObjectId: {ObjectId}. Scopes: {Scopes}. Roles: {Roles}",
                context.Request.Method,
                context.Request.Path,
                context.User.Identity?.Name ?? "unknown",
                context.User.GetObjectId(),
                string.Join(", ", context.User.GetScopes()),
                string.Join(", ", context.User.GetRoles()));
        }

        await _next(context);
    }
}
