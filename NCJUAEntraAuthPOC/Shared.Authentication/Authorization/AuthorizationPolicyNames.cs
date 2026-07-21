namespace Shared.Authentication.Authorization;

/// <summary>
/// Defines policy names used by every microservice.
/// </summary>
public static class AuthorizationPolicyNames
{
    /// <summary>
    /// Policy requiring the <c>order.read</c> scope.
    /// </summary>
    public const string OrderRead = "Order.Read";

    /// <summary>
    /// Policy requiring the <c>order.write</c> scope.
    /// </summary>
    public const string OrderWrite = "Order.Write";

    /// <summary>
    /// Policy requiring the <c>Admin</c> app role.
    /// </summary>
    public const string AdminOnly = "AdminOnly";
}
