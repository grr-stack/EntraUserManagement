namespace Auth.Service.Infrastructure.Abstractions;

/// <summary>
/// Obtains application tokens for Microsoft Graph.
/// </summary>
public interface IGraphAccessTokenProvider
{
    /// <summary>
    /// Returns an access token for Microsoft Graph.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The bearer token.</returns>
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken);
}
