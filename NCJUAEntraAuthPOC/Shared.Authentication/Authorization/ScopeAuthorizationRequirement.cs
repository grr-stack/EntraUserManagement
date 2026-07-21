using Microsoft.AspNetCore.Authorization;

namespace Shared.Authentication.Authorization;

/// <summary>
/// Represents an authorization requirement based on delegated OAuth scopes.
/// </summary>
public sealed class ScopeAuthorizationRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ScopeAuthorizationRequirement"/> class.
    /// </summary>
    /// <param name="requiredScopes">The scopes that satisfy the requirement.</param>
    public ScopeAuthorizationRequirement(params string[] requiredScopes)
    {
        RequiredScopes = requiredScopes;
    }

    /// <summary>
    /// Gets the accepted scopes.
    /// </summary>
    public IReadOnlyCollection<string> RequiredScopes { get; }
}
