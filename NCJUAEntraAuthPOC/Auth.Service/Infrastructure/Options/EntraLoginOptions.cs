namespace Auth.Service.Infrastructure.Options;

/// <summary>
/// Configuration for Microsoft Entra interactive sign-in.
/// </summary>
public sealed class EntraLoginOptions
{
    /// <summary>
    /// The configuration section name.
    /// </summary>
    public const string SectionName = "EntraLogin";

    /// <summary>
    /// Gets or sets the confidential client identifier. Falls back to AzureAd:ClientId when omitted.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the confidential client secret. Keep this outside source control.
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the default redirect URI registered in Microsoft Entra ID.
    /// </summary>
    public string DefaultRedirectUri { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the scopes requested during interactive login.
    /// </summary>
    public List<string> Scopes { get; set; } = [];
}
