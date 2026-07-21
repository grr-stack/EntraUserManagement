using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Auth.Service.Application.Abstractions;
using Auth.Service.Infrastructure.Abstractions;
using Auth.Service.Infrastructure.Exceptions;
using Auth.Service.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Shared.Contracts.Auth;

namespace Auth.Service.Application.Services;

/// <summary>
/// Creates users in Microsoft Entra ID using Microsoft Graph.
/// </summary>
public sealed class UserRegistrationService : IUserRegistrationService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly HttpClient _httpClient;
    private readonly IGraphAccessTokenProvider _tokenProvider;
    private readonly GraphProvisioningOptions _options;
    private readonly ILogger<UserRegistrationService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserRegistrationService"/> class.
    /// </summary>
    public UserRegistrationService(
        HttpClient httpClient,
        IGraphAccessTokenProvider tokenProvider,
        IOptions<GraphProvisioningOptions> options,
        ILogger<UserRegistrationService> logger)
    {
        _httpClient = httpClient;
        _tokenProvider = tokenProvider;
        _options = options.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<RegisteredUserResponse> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var userPrincipalName = BuildUserPrincipalName(request);
        var accessToken = await _tokenProvider.GetAccessTokenAsync(cancellationToken);

        var graphRequest = new
        {
            accountEnabled = request.AccountEnabled,
            displayName = request.DisplayName.Trim(),
            givenName = request.GivenName?.Trim(),
            surname = request.Surname?.Trim(),
            mailNickname = request.UserName.Trim(),
            userPrincipalName,
            otherMails = new[] { request.Email.Trim() },
            passwordProfile = new
            {
                password = request.TemporaryPassword,
                forceChangePasswordNextSignIn = request.ForceChangePasswordNextSignIn
            }
        };

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "users");
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        httpRequest.Content = new StringContent(
            JsonSerializer.Serialize(graphRequest, SerializerOptions),
            Encoding.UTF8,
            "application/json");

        using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        var payload = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Microsoft Graph user creation failed. Status: {StatusCode}. Body: {Body}", (int)response.StatusCode, payload);

            var statusCode = response.StatusCode switch
            {
                System.Net.HttpStatusCode.BadRequest => StatusCodes.Status400BadRequest,
                System.Net.HttpStatusCode.Conflict => StatusCodes.Status409Conflict,
                System.Net.HttpStatusCode.Unauthorized => StatusCodes.Status502BadGateway,
                System.Net.HttpStatusCode.Forbidden => StatusCodes.Status502BadGateway,
                _ => StatusCodes.Status502BadGateway
            };

            throw new GraphProvisioningException("Microsoft Graph rejected the user registration request.", statusCode, payload);
        }

        using var document = JsonDocument.Parse(payload);
        var root = document.RootElement;

        return new RegisteredUserResponse
        {
            Id = root.GetProperty("id").GetString() ?? string.Empty,
            UserPrincipalName = root.GetProperty("userPrincipalName").GetString() ?? userPrincipalName,
            DisplayName = root.GetProperty("displayName").GetString() ?? request.DisplayName,
            Email = root.TryGetProperty("mail", out var mailElement) && !string.IsNullOrWhiteSpace(mailElement.GetString())
                ? mailElement.GetString()!
                : request.Email,
            AccountEnabled = root.TryGetProperty("accountEnabled", out var enabledElement) && enabledElement.ValueKind is JsonValueKind.True or JsonValueKind.False
                ? enabledElement.GetBoolean()
                : request.AccountEnabled
        };
    }

    private string BuildUserPrincipalName(RegisterUserRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.UserPrincipalName))
        {
            return request.UserPrincipalName.Trim();
        }

        if (string.IsNullOrWhiteSpace(_options.DefaultDomain))
        {
            throw new GraphProvisioningException(
                "GraphProvisioning:DefaultDomain must be configured when UserPrincipalName is not provided.",
                StatusCodes.Status400BadRequest);
        }

        return $"{request.UserName.Trim()}@{_options.DefaultDomain.Trim()}";
    }
}
