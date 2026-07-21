using Shared.Contracts.Auth;

namespace Auth.Service.Application.Abstractions;

/// <summary>
/// Provisions users in Microsoft Entra ID for this application.
/// </summary>
public interface IUserRegistrationService
{
    /// <summary>
    /// Creates a new Microsoft Entra user.
    /// </summary>
    /// <param name="request">The registration request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created user.</returns>
    Task<RegisteredUserResponse> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken);
}
