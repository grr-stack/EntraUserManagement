namespace Shared.Contracts.Auth;

/// <summary>
/// Represents the current authenticated user.
/// </summary>
public sealed class UserProfileResponse
{
    /// <summary>
    /// Gets or sets the Microsoft Entra object identifier.
    /// </summary>
    public required string ObjectId { get; init; }

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets or sets the preferred email address.
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// Gets or sets the app roles assigned to the caller.
    /// </summary>
    public required IReadOnlyCollection<string> Roles { get; init; }

    /// <summary>
    /// Gets or sets the delegated scopes assigned to the caller.
    /// </summary>
    public required IReadOnlyCollection<string> Scopes { get; init; }

    /// <summary>
    /// Gets or sets the flattened claims list.
    /// </summary>
    public required IReadOnlyCollection<ClaimResponse> Claims { get; init; }
}
