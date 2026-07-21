using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.DataProtection;
using Order.Service.Application.Abstractions;
using Order.Service.Application.Services;
using Order.Service.Infrastructure.ExceptionHandling;
using Order.Service.Infrastructure.Repositories;
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
builder.Services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
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
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    var swaggerOptions = app.Services.GetRequiredService<IOptions<SwaggerOAuthOptions>>().Value;
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Order.Service v1");
    options.OAuthClientId(swaggerOptions.ClientId);
    if (swaggerOptions.UsePkce)
    {
        options.OAuthUsePkce();
    }

    if (swaggerOptions.Scopes.Count > 0)
    {
        options.OAuthScopes(swaggerOptions.Scopes.ToArray());
    }
});
app.UseAuthentication();
app.UseUserContextLogging();
app.UseAuthorization();

app.MapControllers();

app.Run();
