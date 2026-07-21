using Microsoft.AspNetCore.Authorization;

namespace Shared.Authentication.Authorization;

/// <summary>
/// Authorizes access when a caller contains at least one required delegated scope.
/// </summary>
public sealed class ScopeAuthorizationHandler : AuthorizationHandler<ScopeAuthorizationRequirement>
{
    /// <inheritdoc />
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ScopeAuthorizationRequirement requirement)
    {
        var scopes = context.User.FindFirst("scp")?.Value
            ?.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            ?? Array.Empty<string>();

        if (scopes.Any(scope => requirement.RequiredScopes.Contains(scope, StringComparer.OrdinalIgnoreCase)))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
