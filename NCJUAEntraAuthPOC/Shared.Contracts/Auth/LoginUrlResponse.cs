namespace Shared.Contracts.Auth;

/// <summary>
/// Represents the Microsoft Entra sign-in URL and related client state.
/// </summary>
public sealed class LoginUrlResponse
{
    /// <summary>
    /// Gets or sets the URL that the client should open to authenticate.
    /// </summary>
    public required string AuthorizationUrl { get; init; }

    /// <summary>
    /// Gets or sets the redirect URI that will receive the authorization code.
    /// </summary>
    public required string RedirectUri { get; init; }

    /// <summary>
    /// Gets or sets the state value that should be verified by the client.
    /// </summary>
    public required string State { get; init; }

    /// <summary>
    /// Gets or sets the requested scopes.
    /// </summary>
    public required IReadOnlyCollection<string> Scopes { get; init; }
}
