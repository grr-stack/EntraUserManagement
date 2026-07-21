namespace Auth.Service.Infrastructure.Exceptions;

/// <summary>
/// Represents a Microsoft Graph provisioning failure.
/// </summary>
public sealed class GraphProvisioningException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GraphProvisioningException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code that should be returned.</param>
    /// <param name="detail">The detail payload.</param>
    public GraphProvisioningException(string message, int statusCode, string? detail = null)
        : base(message)
    {
        StatusCode = statusCode;
        Detail = detail;
    }

    /// <summary>
    /// Gets the HTTP status code that should be surfaced by the API.
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// Gets the provider-specific error detail.
    /// </summary>
    public string? Detail { get; }
}
