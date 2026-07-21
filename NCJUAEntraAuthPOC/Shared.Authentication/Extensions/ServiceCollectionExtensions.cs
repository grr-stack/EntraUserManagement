using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Shared.Authentication.Authorization;
using Shared.Authentication.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Shared.Authentication.Extensions;

/// <summary>
/// Registers shared authentication, authorization, and Swagger dependencies.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Microsoft Entra ID JWT bearer authentication.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddSharedAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<AzureAdOptions>()
            .Bind(configuration.GetSection(AzureAdOptions.SectionName))
            .Validate(options => !string.IsNullOrWhiteSpace(options.TenantId), "AzureAd:TenantId is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.ClientId), "AzureAd:ClientId is required.")
            .ValidateOnStart();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(
                configuration.GetSection(AzureAdOptions.SectionName),
                JwtBearerDefaults.AuthenticationScheme,
                subscribeToJwtBearerMiddlewareDiagnosticsEvents: false);

        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<AzureAdOptions>, ILoggerFactory>((options, azureAdOptions, loggerFactory) =>
        {
            var logger = loggerFactory.CreateLogger("SharedAuthentication");
            var azureAd = azureAdOptions.Value;
            var validAudience = string.IsNullOrWhiteSpace(azureAd.Audience) ? azureAd.ClientId : azureAd.Audience;
            var validAudiences = new[]
            {
                validAudience,
                azureAd.ClientId,
                $"api://{azureAd.ClientId}"
            }.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = $"{azureAd.Instance.TrimEnd('/')}/{azureAd.TenantId}/v2.0",
                ValidIssuers =
                [
                    $"{azureAd.Instance.TrimEnd('/')}/{azureAd.TenantId}/v2.0",
                    $"{azureAd.Instance.TrimEnd('/')}/{azureAd.TenantId}/"
                ],
                ValidAudience = validAudience,
                ValidAudiences = validAudiences,
                NameClaimType = "name",
                RoleClaimType = "roles"
            };

            options.Events ??= new JwtBearerEvents();
            options.Events.OnAuthenticationFailed = context =>
            {
                logger.LogError(context.Exception, "Authentication failed for {Path}", context.Request.Path);
                return Task.CompletedTask;
            };
            options.Events.OnChallenge = context =>
            {
                logger.LogWarning(
                    "Authentication challenge triggered for {Path}. Error: {Error}. Description: {Description}",
                    context.Request.Path,
                    context.Error,
                    context.ErrorDescription);
                return Task.CompletedTask;
            };
            options.Events.OnForbidden = context =>
            {
                logger.LogWarning("Authorization rejected for {Path}", context.Request.Path);
                return Task.CompletedTask;
            };
            options.Events.OnTokenValidated = context =>
            {
                var principal = context.Principal;
                logger.LogInformation(
                    "Token validated for {Name}. ObjectId: {ObjectId}. Scopes: {Scopes}. Roles: {Roles}",
                    principal?.Identity?.Name ?? "unknown",
                    principal?.GetObjectId() ?? string.Empty,
                    string.Join(", ", principal?.GetScopes() ?? Array.Empty<string>()),
                    string.Join(", ", principal?.GetRoles() ?? Array.Empty<string>()));
                return Task.CompletedTask;
            };
        });

        return services;
    }

    /// <summary>
    /// Adds reusable authorization policies based on scopes and roles.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddSharedAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthorizationPolicyNames.OrderRead, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.Requirements.Add(new Authorization.ScopeAuthorizationRequirement("order.read"));
            });

            options.AddPolicy(AuthorizationPolicyNames.OrderWrite, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.Requirements.Add(new Authorization.ScopeAuthorizationRequirement("order.write"));
            });

            options.AddPolicy(AuthorizationPolicyNames.AdminOnly, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole("Admin");
            });
        });

        services.TryAddEnumerable(ServiceDescriptor.Singleton<IAuthorizationHandler, Authorization.ScopeAuthorizationHandler>());
        services.TryAddSingleton<IAuthorizationMiddlewareResultHandler, Authorization.LoggingAuthorizationMiddlewareResultHandler>();

        return services;
    }

    /// <summary>
    /// Configures Swagger to authenticate against Microsoft Entra ID.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddSwaggerAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();

        services.AddOptions<SwaggerOAuthOptions>()
            .Bind(configuration.GetSection(SwaggerOAuthOptions.SectionName))
            .ValidateOnStart();

        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerGenOptions>();
        services.AddSwaggerGen();

        return services;
    }

    private sealed class ConfigureSwaggerGenOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly AzureAdOptions _azureAdOptions;
        private readonly SwaggerOAuthOptions _swaggerOAuthOptions;

        public ConfigureSwaggerGenOptions(
            IOptions<AzureAdOptions> azureAdOptions,
            IOptions<SwaggerOAuthOptions> swaggerOAuthOptions)
        {
            _azureAdOptions = azureAdOptions.Value;
            _swaggerOAuthOptions = swaggerOAuthOptions.Value;
        }

        public void Configure(SwaggerGenOptions options)
        {
            var authority = $"{_azureAdOptions.Instance.TrimEnd('/')}/{_azureAdOptions.TenantId}/oauth2/v2.0";
            var scopes = (_swaggerOAuthOptions.Scopes.Count > 0
                    ? _swaggerOAuthOptions.Scopes
                    : GetDefaultScopes(_azureAdOptions))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToDictionary(scope => scope, scope => $"Access scope {scope}");

            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = Assembly.GetEntryAssembly()?.GetName().Name ?? "Protected API",
                Version = "v1",
                Description = "JWT Bearer protected microservice backed by Microsoft Entra ID."
            });

            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"{authority}/authorize"),
                        TokenUrl = new Uri($"{authority}/token"),
                        Scopes = scopes
                    }
                },
                Description = "OAuth2 PKCE Authorization Code Flow"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "oauth2"
                        }
                    },
                    scopes.Keys.ToArray()
                }
            });

            var xmlFile = $"{Assembly.GetEntryAssembly()?.GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }
        }

        private static IEnumerable<string> GetDefaultScopes(AzureAdOptions azureAdOptions)
        {
            var audience = string.IsNullOrWhiteSpace(azureAdOptions.Audience)
                ? $"api://{azureAdOptions.ClientId}"
                : azureAdOptions.Audience;

            yield return $"{audience}/order.read";
            yield return $"{audience}/order.write";
        }
    }
}
