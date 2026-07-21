using System.ComponentModel.DataAnnotations;

namespace Shared.Contracts.Auth;

/// <summary>
/// Represents an authorization-code exchange request.
/// </summary>
public sealed class LoginRequest
{
    /// <summary>
    /// Gets or sets the authorization code returned by Microsoft Entra ID.
    /// </summary>
    [Required]
    public string Code { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the redirect URI used during login.
    /// </summary>
    [MaxLength(2048)]
    public string? RedirectUri { get; init; }
}
