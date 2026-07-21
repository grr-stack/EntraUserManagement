using System.Net.Http.Headers;
using System.Text.Json;
using Auth.Service.Infrastructure.Abstractions;
using Auth.Service.Infrastructure.Exceptions;
using Auth.Service.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Shared.Authentication.Options;

namespace Auth.Service.Infrastructure.Services;

/// <summary>
/// Acquires Microsoft Graph app-only access tokens using client credentials.
/// </summary>
public sealed class GraphAccessTokenProvider : IGraphAccessTokenProvider
{
    private readonly HttpClient _httpClient;
    private readonly GraphProvisioningOptions _graphOptions;
    private readonly AzureAdOptions _azureAdOptions;
    private readonly ILogger<GraphAccessTokenProvider> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphAccessTokenProvider"/> class.
    /// </summary>
    public GraphAccessTokenProvider(
        HttpClient httpClient,
        IOptions<GraphProvisioningOptions> graphOptions,
        IOptions<AzureAdOptions> azureAdOptions,
        ILogger<GraphAccessTokenProvider> logger)
    {
        _httpClient = httpClient;
        _graphOptions = graphOptions.Value;
        _azureAdOptions = azureAdOptions.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        EnsureConfigured();

        var tenantId = string.IsNullOrWhiteSpace(_graphOptions.TenantId)
            ? _azureAdOptions.TenantId
            : _graphOptions.TenantId;

        var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token")
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = _graphOptions.ClientId,
                ["client_secret"] = _graphOptions.ClientSecret,
                ["scope"] = _graphOptions.Scope,
                ["grant_type"] = "client_credentials"
            })
        };

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        var payload = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to acquire Microsoft Graph token. Status: {StatusCode}. Body: {Body}", (int)response.StatusCode, payload);
            throw new GraphProvisioningException("Unable to acquire a Microsoft Graph access token.", StatusCodes.Status502BadGateway, payload);
        }

        using var document = JsonDocument.Parse(payload);
        if (!document.RootElement.TryGetProperty("access_token", out var accessTokenElement))
        {
            throw new GraphProvisioningException("Microsoft Graph token response did not include an access token.", StatusCodes.Status502BadGateway, payload);
        }

        return accessTokenElement.GetString() ?? throw new GraphProvisioningException("Microsoft Graph access token was empty.", StatusCodes.Status502BadGateway, payload);
    }

    private void EnsureConfigured()
    {
        if (string.IsNullOrWhiteSpace(_graphOptions.ClientId) || string.IsNullOrWhiteSpace(_graphOptions.ClientSecret))
        {
            throw new GraphProvisioningException(
                "Graph provisioning is not configured. Set GraphProvisioning:ClientId and GraphProvisioning:ClientSecret.",
                StatusCodes.Status503ServiceUnavailable);
        }
    }
}
