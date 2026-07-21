using System.ComponentModel.DataAnnotations;

namespace Shared.Contracts.Auth;

/// <summary>
/// Represents an admin-driven Microsoft Entra user registration request.
/// </summary>
public sealed class RegisterUserRequest
{
    /// <summary>
    /// Gets or sets the user's sign-in alias. Used to construct the UPN when <see cref="UserPrincipalName"/> is omitted.
    /// </summary>
    [Required]
    [MaxLength(64)]
    [RegularExpression("^[A-Za-z0-9._-]+$")]
    public string UserName { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string DisplayName { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the given name.
    /// </summary>
    [MaxLength(100)]
    public string? GivenName { get; init; }

    /// <summary>
    /// Gets or sets the surname.
    /// </summary>
    [MaxLength(100)]
    public string? Surname { get; init; }

    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    [Required]
    [EmailAddress]
    [MaxLength(320)]
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the UPN. When omitted, the service composes one from <see cref="UserName"/> and the configured Entra domain.
    /// </summary>
    [EmailAddress]
    [MaxLength(320)]
    public string? UserPrincipalName { get; init; }

    /// <summary>
    /// Gets or sets the initial temporary password.
    /// </summary>
    [Required]
    [MinLength(8)]
    [MaxLength(256)]
    public string TemporaryPassword { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the user must change password on first sign-in.
    /// </summary>
    public bool ForceChangePasswordNextSignIn { get; init; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the new account is enabled.
    /// </summary>
    public bool AccountEnabled { get; init; } = true;
}
