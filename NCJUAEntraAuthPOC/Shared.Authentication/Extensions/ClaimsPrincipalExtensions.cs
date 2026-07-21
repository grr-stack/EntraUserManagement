using System.Security.Claims;

namespace Shared.Authentication.Extensions;

/// <summary>
/// Provides helper methods for reading common Microsoft Entra claims.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Gets the Entra object identifier from the current principal.
    /// </summary>
    /// <param name="principal">The principal.</param>
    /// <returns>The object identifier when present.</returns>
    public static string GetObjectId(this ClaimsPrincipal principal)
        => principal.FindFirstValue("oid")
            ?? principal.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier")
            ?? string.Empty;

    /// <summary>
    /// Gets the caller email from standard Entra claims.
    /// </summary>
    /// <param name="principal">The principal.</param>
    /// <returns>The preferred email when present.</returns>
    public static string GetEmail(this ClaimsPrincipal principal)
        => principal.FindFirstValue(ClaimTypes.Email)
            ?? principal.FindFirstValue("preferred_username")
            ?? principal.FindFirstValue("upn")
            ?? string.Empty;

    /// <summary>
    /// Gets all delegated scopes from the caller.
    /// </summary>
    /// <param name="principal">The principal.</param>
    /// <returns>The scopes.</returns>
    public static IReadOnlyCollection<string> GetScopes(this ClaimsPrincipal principal)
        => principal.FindFirst("scp")?.Value
            ?.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            ?? Array.Empty<string>();

    /// <summary>
    /// Gets all roles from the caller.
    /// </summary>
    /// <param name="principal">The principal.</param>
    /// <returns>The roles.</returns>
    public static IReadOnlyCollection<string> GetRoles(this ClaimsPrincipal principal)
        => principal.FindAll("roles").Select(claim => claim.Value).ToArray();

    private static string? FindFirstValue(this ClaimsPrincipal principal, string claimType)
        => principal.FindFirst(claimType)?.Value;
}
