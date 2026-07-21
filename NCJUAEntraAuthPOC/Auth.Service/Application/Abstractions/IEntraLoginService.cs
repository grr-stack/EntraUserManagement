using Shared.Contracts.Auth;

namespace Auth.Service.Application.Abstractions;

/// <summary>
/// Handles Microsoft Entra interactive login flows for API clients.
/// </summary>
public interface IEntraLoginService
{
    /// <summary>
    /// Builds the authorization URL that the client should open to sign in.
    /// </summary>
    /// <param name="redirectUri">The callback URI registered in Microsoft Entra ID.</param>
    /// <param name="state">An optional caller-provided state token.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The authorization URL payload.</returns>
    Task<LoginUrlResponse> BuildLoginUrlAsync(string? redirectUri, string? state, CancellationToken cancellationToken);

    /// <summary>
    /// Exchanges an authorization code for Microsoft Entra tokens.
    /// </summary>
    /// <param name="request">The token exchange request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The token response.</returns>
    Task<LoginResponse> ExchangeCodeAsync(LoginRequest request, CancellationToken cancellationToken);
}
