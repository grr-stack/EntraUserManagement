using System.Text.Json.Serialization;

namespace Shared.Contracts.Auth;

/// <summary>
/// Represents the token response from Microsoft Entra ID.
/// </summary>
public sealed class LoginResponse
{
    /// <summary>
    /// Gets or sets the token type.
    /// </summary>
    [JsonPropertyName("token_type")]
    public string TokenType { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the granted scope string.
    /// </summary>
    [JsonPropertyName("scope")]
    public string Scope { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the access token.
    /// </summary>
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the refresh token when offline access is granted.
    /// </summary>
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; init; }

    /// <summary>
    /// Gets or sets the ID token when requested.
    /// </summary>
    [JsonPropertyName("id_token")]
    public string? IdToken { get; init; }

    /// <summary>
    /// Gets or sets the access token lifetime in seconds.
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }
}
