namespace Auth.Service.Infrastructure.Options;

/// <summary>
/// Configuration required to provision users through Microsoft Graph.
/// </summary>
public sealed class GraphProvisioningOptions
{
    /// <summary>
    /// The configuration section name.
    /// </summary>
    public const string SectionName = "GraphProvisioning";

    /// <summary>
    /// Gets or sets the tenant identifier used for client credentials.
    /// </summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the client identifier for the confidential application used to call Microsoft Graph.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the client secret. Keep this outside source control.
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Microsoft Graph scope.
    /// </summary>
    public string Scope { get; set; } = "https://graph.microsoft.com/.default";

    /// <summary>
    /// Gets or sets the Microsoft Graph base URL.
    /// </summary>
    public string BaseUrl { get; set; } = "https://graph.microsoft.com/v1.0/";

    /// <summary>
    /// Gets or sets the default verified domain used to compose UPN values.
    /// </summary>
    public string DefaultDomain { get; set; } = string.Empty;
}
