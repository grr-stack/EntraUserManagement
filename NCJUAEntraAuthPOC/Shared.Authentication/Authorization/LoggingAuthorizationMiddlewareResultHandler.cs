using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.Extensions.Logging;

namespace Shared.Authentication.Authorization;

/// <summary>
/// Logs authorization failures before the framework writes the response.
/// </summary>
public sealed class LoggingAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();
    private readonly ILogger<LoggingAuthorizationMiddlewareResultHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingAuthorizationMiddlewareResultHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public LoggingAuthorizationMiddlewareResultHandler(ILogger<LoggingAuthorizationMiddlewareResultHandler> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        if (authorizeResult.Forbidden)
        {
            _logger.LogWarning(
                "Authorization failed for {Path}. User: {User}. Requirements: {Requirements}",
                context.Request.Path,
                context.User.Identity?.Name ?? "anonymous",
                string.Join(", ", policy.Requirements.Select(requirement => requirement.GetType().Name)));
        }

        await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }
}
