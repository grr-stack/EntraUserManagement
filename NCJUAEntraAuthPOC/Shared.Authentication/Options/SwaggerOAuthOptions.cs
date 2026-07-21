namespace Shared.Authentication.Options;

/// <summary>
/// Strongly typed Swagger OAuth settings.
/// </summary>
public sealed class SwaggerOAuthOptions
{
    /// <summary>
    /// The configuration section name.
    /// </summary>
    public const string SectionName = "SwaggerOAuth";

    /// <summary>
    /// Gets or sets the Swagger client identifier.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether PKCE is enabled.
    /// </summary>
    public bool UsePkce { get; set; } = true;

    /// <summary>
    /// Gets or sets the scopes requested by Swagger UI.
    /// </summary>
    public List<string> Scopes { get; set; } = [];
}
