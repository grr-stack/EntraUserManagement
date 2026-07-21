using Microsoft.AspNetCore.Builder;
using Shared.Authentication.Middleware;

namespace Shared.Authentication.Extensions;

/// <summary>
/// Provides shared middleware extensions.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds request logging for authenticated user context.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder.</returns>
    public static IApplicationBuilder UseUserContextLogging(this IApplicationBuilder app)
        => app.UseMiddleware<UserContextLoggingMiddleware>();
}
