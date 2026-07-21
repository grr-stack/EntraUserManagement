using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Order.Api.Security;

public static class AuthorizationPolicyExtensions
{
    public static AuthorizationPolicyBuilder RequireScopeOrAppRole(
        this AuthorizationPolicyBuilder policyBuilder,
        string requiredScope,
        string requiredRole)
    {
        return policyBuilder.RequireAssertion(context =>
        {
            var scopes = context.User.FindFirstValue("scp")
                ?.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                ?? [];

            var roles = context.User.FindAll(ClaimTypes.Role)
                .Select(claim => claim.Value)
                .Concat(context.User.FindAll("roles").Select(claim => claim.Value));

            return scopes.Contains(requiredScope, StringComparer.OrdinalIgnoreCase)
                || roles.Contains(requiredRole, StringComparer.OrdinalIgnoreCase);
        });
    }
}
