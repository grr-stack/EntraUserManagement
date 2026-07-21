using System.Net.Http.Headers;
using System.Text.Json;
using Auth.Service.Application.Abstractions;
using Auth.Service.Infrastructure.Exceptions;
using Auth.Service.Infrastructure.Options;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Shared.Authentication.Options;
using Shared.Contracts.Auth;

namespace Auth.Service.Infrastructure.Services;

/// <summary>
/// Implements Microsoft Entra authorization-code login for confidential clients.
/// </summary>
public sealed class EntraLoginService : IEntraLoginService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly HttpClient _httpClient;
    private readonly AzureAdOptions _azureAdOptions;
    private readonly EntraLoginOptions _loginOptions;
    private readonly ILogger<EntraLoginService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EntraLoginService"/> class.
    /// </summary>
    public EntraLoginService(
        HttpClient httpClient,
        IOptions<AzureAdOptions> azureAdOptions,
        IOptions<EntraLoginOptions> loginOptions,
        ILogger<EntraLoginService> logger)
    {
        _httpClient = httpClient;
        _azureAdOptions = azureAdOptions.Value;
        _loginOptions = loginOptions.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public Task<LoginUrlResponse> BuildLoginUrlAsync(string? redirectUri, string? state, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var resolvedRedirectUri = ResolveRedirectUri(redirectUri);
        var resolvedState = string.IsNullOrWhiteSpace(state) ? Guid.NewGuid().ToString("N") : state.Trim();
        var scopes = ResolveScopes();
        var authority = $"{_azureAdOptions.Instance.TrimEnd('/')}/{_azureAdOptions.TenantId}/oauth2/v2.0/authorize";

        var url = QueryHelpers.AddQueryString(authority, new Dictionary<string, string?>
        {
            ["client_id"] = ResolveClientId(),
            ["response_type"] = "code",
            ["redirect_uri"] = resolvedRedirectUri,
            ["response_mode"] = "query",
            ["scope"] = string.Join(' ', scopes),
            ["state"] = resolvedState
        });

        return Task.FromResult(new LoginUrlResponse
        {
            AuthorizationUrl = url,
            RedirectUri = resolvedRedirectUri,
            State = resolvedState,
            Scopes = scopes
        });
    }

    /// <inheritdoc />
    public async Task<LoginResponse> ExchangeCodeAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        EnsureConfigured();

        var resolvedRedirectUri = ResolveRedirectUri(request.RedirectUri);
        var tokenEndpoint = $"{_azureAdOptions.Instance.TrimEnd('/')}/{_azureAdOptions.TenantId}/oauth2/v2.0/token";

        using var tokenRequest = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint)
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = ResolveClientId(),
                ["client_secret"] = _loginOptions.ClientSecret,
                ["grant_type"] = "authorization_code",
                ["code"] = request.Code.Trim(),
                ["redirect_uri"] = resolvedRedirectUri,
                ["scope"] = string.Join(' ', ResolveScopes())
            })
        };

        tokenRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var response = await _httpClient.SendAsync(tokenRequest, cancellationToken);
        var payload = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Microsoft Entra token exchange failed. Status: {StatusCode}. Body: {Body}", (int)response.StatusCode, payload);
            throw new GraphProvisioningException("Microsoft Entra rejected the login request.", StatusCodes.Status502BadGateway, payload);
        }

        var tokenResponse = JsonSerializer.Deserialize<LoginResponse>(payload, SerializerOptions);
        if (tokenResponse is null || string.IsNullOrWhiteSpace(tokenResponse.AccessToken))
        {
            throw new GraphProvisioningException("Microsoft Entra token response did not include an access token.", StatusCodes.Status502BadGateway, payload);
        }

        return tokenResponse;
    }

    private void EnsureConfigured()
    {
        if (string.IsNullOrWhiteSpace(ResolveClientId()) || string.IsNullOrWhiteSpace(_loginOptions.ClientSecret))
        {
            throw new GraphProvisioningException(
                "Interactive login is not configured. Set EntraLogin:ClientSecret and ensure a client id is available.",
                StatusCodes.Status503ServiceUnavailable);
        }
    }

    private string ResolveClientId()
        => string.IsNullOrWhiteSpace(_loginOptions.ClientId)
            ? _azureAdOptions.ClientId
            : _loginOptions.ClientId;

    private string ResolveRedirectUri(string? redirectUri)
    {
        var resolvedRedirectUri = string.IsNullOrWhiteSpace(redirectUri)
            ? _loginOptions.DefaultRedirectUri
            : redirectUri.Trim();

        if (string.IsNullOrWhiteSpace(resolvedRedirectUri))
        {
            throw new GraphProvisioningException(
                "A redirect URI is required. Provide it in the request or configure EntraLogin:DefaultRedirectUri.",
                StatusCodes.Status400BadRequest);
        }

        return resolvedRedirectUri;
    }

    private IReadOnlyList<string> ResolveScopes()
    {
        var scopes = _loginOptions.Scopes
            .Where(scope => !string.IsNullOrWhiteSpace(scope))
            .Select(scope => scope.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return scopes.Length > 0
            ? scopes
            : ["openid", "profile", "offline_access"];
    }
}
