namespace Shared.Authentication.Options;

/// <summary>
/// Strongly typed Microsoft Entra settings.
/// </summary>
public sealed class AzureAdOptions
{
    /// <summary>
    /// The configuration section name.
    /// </summary>
    public const string SectionName = "AzureAd";

    /// <summary>
    /// Gets or sets the authority base URL.
    /// </summary>
    public string Instance { get; set; } = "https://login.microsoftonline.com/";

    /// <summary>
    /// Gets or sets the tenant identifier.
    /// </summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the API application client identifier.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the tenant domain.
    /// </summary>
    public string Domain { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the audience or application ID URI.
    /// </summary>
    public string Audience { get; set; } = string.Empty;
}
