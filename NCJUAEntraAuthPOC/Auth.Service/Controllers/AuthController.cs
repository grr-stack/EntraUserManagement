using Auth.Service.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Authentication.Extensions;
using Shared.Contracts.Auth;

namespace Auth.Service.Controllers;

/// <summary>
/// Exposes authenticated user profile information sourced from Microsoft Entra ID claims.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IEntraLoginService _entraLoginService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    public AuthController(IEntraLoginService entraLoginService)
    {
        _entraLoginService = entraLoginService;
    }

    /// <summary>
    /// Returns the Microsoft Entra authorization URL for interactive login.
    /// </summary>
    /// <param name="redirectUri">Optional redirect URI. Falls back to configuration when omitted.</param>
    /// <param name="state">Optional caller state to round-trip through Entra.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The authorization URL.</returns>
    [HttpGet("login-url")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginUrlResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoginUrlResponse>> GetLoginUrlAsync(
        [FromQuery] string? redirectUri,
        [FromQuery] string? state,
        CancellationToken cancellationToken)
    {
        var response = await _entraLoginService.BuildLoginUrlAsync(redirectUri, state, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Exchanges a Microsoft Entra authorization code for tokens.
    /// </summary>
    /// <param name="request">The authorization-code exchange request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The issued access token payload.</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<LoginResponse>> LoginAsync(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _entraLoginService.ExchangeCodeAsync(request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Returns the current authenticated user's profile.
    /// </summary>
    /// <returns>The caller profile.</returns>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserProfileResponse>> GetMeAsync()
    {
        var claims = User.Claims
            .OrderBy(claim => claim.Type, StringComparer.Ordinal)
            .Select(claim => new ClaimResponse(claim.Type, claim.Value))
            .ToArray();

        var response = new UserProfileResponse
        {
            ObjectId = User.GetObjectId(),
            Name = User.Identity?.Name ?? string.Empty,
            Email = User.GetEmail(),
            Roles = User.GetRoles(),
            Scopes = User.GetScopes(),
            Claims = claims
        };

        return await Task.FromResult(Ok(response));
    }

    /// <summary>
    /// Returns every claim present in the caller's JWT.
    /// </summary>
    /// <returns>The claim collection.</returns>
    [HttpGet("claims")]
    [Authorize]
    [ProducesResponseType(typeof(IReadOnlyCollection<ClaimResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyCollection<ClaimResponse>>> GetClaimsAsync()
    {
        var claims = User.Claims
            .OrderBy(claim => claim.Type, StringComparer.Ordinal)
            .Select(claim => new ClaimResponse(claim.Type, claim.Value))
            .ToArray();

        return await Task.FromResult(Ok(claims));
    }

    /// <summary>
    /// Returns a simple health response.
    /// </summary>
    /// <returns>A healthy service response.</returns>
    [HttpGet("health")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHealthAsync()
        => await Task.FromResult(Ok(new { status = "OK" }));
}
