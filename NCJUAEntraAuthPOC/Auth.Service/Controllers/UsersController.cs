using Auth.Service.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Authentication.Authorization;
using Shared.Contracts.Auth;

namespace Auth.Service.Controllers;

/// <summary>
/// Provisions application users in Microsoft Entra ID.
/// </summary>
[ApiController]
[Route("api/[controller]")]
//[Authorize(Policy = AuthorizationPolicyNames.AdminOnly)]
public sealed class UsersController : ControllerBase
{
    private readonly IUserRegistrationService _userRegistrationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UsersController"/> class.
    /// </summary>
    public UsersController(IUserRegistrationService userRegistrationService)
    {
        _userRegistrationService = userRegistrationService;
    }

    /// <summary>
    /// Registers a user in Microsoft Entra ID for this application.
    /// </summary>
    /// <param name="request">The registration request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created user.</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisteredUserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<RegisteredUserResponse>> RegisterAsync(
        [FromBody] RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        var createdUser = await _userRegistrationService.RegisterAsync(request, cancellationToken);

        // CreatedAtAction was pointing to this same POST action which has no route parameter for "id",
        // causing URL generation failures and a 500 response. Return a Created result with an explicit
        // location instead.
        var location = $"/api/Users/{createdUser.Id}";
        return Created(location, createdUser);
    }
}
