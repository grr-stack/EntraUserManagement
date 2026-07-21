using Auth.Service.Application.Abstractions;
using Auth.Service.Application.Services;
using Auth.Service.Infrastructure.Abstractions;
using Auth.Service.Infrastructure.ExceptionHandling;
using Auth.Service.Infrastructure.Options;
using Auth.Service.Infrastructure.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using Shared.Authentication.Extensions;
using Shared.Authentication.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, ".keys")));

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddControllers();
builder.Services.Configure<GraphProvisioningOptions>(builder.Configuration.GetSection(GraphProvisioningOptions.SectionName));
builder.Services.Configure<EntraLoginOptions>(builder.Configuration.GetSection(EntraLoginOptions.SectionName));
builder.Services.AddHttpClient<IGraphAccessTokenProvider, GraphAccessTokenProvider>();
builder.Services.AddHttpClient<IEntraLoginService, EntraLoginService>();
builder.Services.AddHttpClient<IUserRegistrationService, UserRegistrationService>((serviceProvider, client) =>
{
    var graphOptions = serviceProvider.GetRequiredService<IOptions<GraphProvisioningOptions>>().Value;
    client.BaseAddress = new Uri(graphOptions.BaseUrl);
});

// Allow services to read the current HTTP context (used to optionally forward an incoming bearer token)
builder.Services.AddHttpContextAccessor();

// Add CORS to allow Swagger UI and frontend applications
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSwaggerUI", policy =>
    {
        policy
            .WithOrigins("https://localhost:7101", "https://localhost:3000", "https://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Services.AddSharedAuthentication(builder.Configuration);
builder.Services.AddSharedAuthorization();
builder.Services.AddSwaggerAuthentication(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors("AllowSwaggerUI");
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    var swaggerOptions = app.Services.GetRequiredService<IOptions<SwaggerOAuthOptions>>().Value;
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth.Service v1");
    options.OAuthClientId(swaggerOptions.ClientId);
    options.OAuthScopes(swaggerOptions.Scopes.ToArray());

    if (swaggerOptions.UsePkce)
    {
        options.OAuthUsePkce();
    }

    // Additional OAuth2 configuration
    options.OAuthAppName("Auth Service API");
    options.OAuthScopeSeparator(" ");
    options.OAuthUseBasicAuthenticationWithAccessCodeGrant();
});
app.UseAuthentication();
app.UseUserContextLogging();
app.UseAuthorization();

app.MapControllers();

app.Run();
