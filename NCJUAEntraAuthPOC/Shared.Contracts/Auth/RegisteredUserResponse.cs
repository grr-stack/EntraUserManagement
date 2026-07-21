namespace Shared.Contracts.Auth;

/// <summary>
/// Represents the result of provisioning a user in Microsoft Entra ID.
/// </summary>
public sealed class RegisteredUserResponse
{
    /// <summary>
    /// Gets or sets the Entra object identifier.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Gets or sets the user principal name.
    /// </summary>
    public required string UserPrincipalName { get; init; }

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    public required string DisplayName { get; init; }

    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the account is enabled.
    /// </summary>
    public bool AccountEnabled { get; init; }
}
