# NCJUAEntraAuthPOC

`NCJUAEntraAuthPOC` is a production-style .NET 8 proof of concept that demonstrates Microsoft Entra ID authentication and authorization across controller-based ASP.NET Core Web API microservices.

## Architecture

The solution contains four projects:

- `Auth.Service`: a profile API that exposes authenticated user information and claims. It also exposes an admin-only Microsoft Entra user provisioning endpoint.
- `Order.Service`: a business API with protected CRUD endpoints.
- `Shared.Authentication`: shared Microsoft Entra ID authentication, authorization policies, Swagger OAuth2 integration, and request logging.
- `Shared.Contracts`: request and response contracts shared between services.

Authentication is performed only by Microsoft Entra ID. Each microservice validates JWT access tokens locally with the shared authentication library. There is no local user store, no ASP.NET Identity, and no database-backed login flow.

## User Creation Boundary

`Auth.Service` does not create users and does not issue tokens. That is intentional. Users must exist in Microsoft Entra ID, and JWT access tokens must be issued by Entra itself.

If you need a test user:

1. Create the user in the Entra admin center or with Microsoft Graph.
2. Assign the `Admin` app role if the user must call `DELETE /api/orders/{id}` or `POST /api/users/register`.
3. Sign in as that user through Swagger, Postman, MSAL, or Azure CLI to obtain a real Entra token.

## Authentication Flow

1. A client application signs in with Microsoft Entra ID.
2. Microsoft Entra ID issues a JWT access token containing scopes and optional app roles.
3. The client sends the bearer token to `Auth.Service` or `Order.Service`.
4. Each service validates the token locally using `Microsoft.Identity.Web`.
5. The request continues only if issuer, audience, lifetime, and signing key validation succeed.

## Authorization Flow

The shared authentication project registers three policies:

- `Order.Read`: requires delegated scope `order.read`.
- `Order.Write`: requires delegated scope `order.write`.
- `AdminOnly`: requires app role `Admin`.

`Auth.Service` applies `AdminOnly` to:

- `POST /api/users/register`

`Order.Service` applies the policies as follows:

- `GET /api/orders`: `Order.Read`
- `GET /api/orders/{id}`: `Order.Read`
- `POST /api/orders`: `Order.Write`
- `PUT /api/orders/{id}`: `Order.Write`
- `DELETE /api/orders/{id}`: `AdminOnly`

## Solution Structure

```text
NCJUAEntraAuthPOC/
|-- Auth.Service/
|-- Order.Service/
|-- Shared.Authentication/
|-- Shared.Contracts/
`-- README.md
```

Within each service, the sample uses a lightweight clean architecture split across:

- `Controllers`
- `Application`
- `Domain`
- `Infrastructure`

## Entra Configuration

The sample uses the following Microsoft Entra identifiers:

- `TenantId`: `ce0aea9c-1d39-404d-a541-3be93b548227`
- `ClientId`: `6344c055-0b0e-430a-a1c8-e743d8a3b219`

All Entra settings live in `appsettings.json` and `appsettings.Development.json`.

Example configuration:

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "ce0aea9c-1d39-404d-a541-3be93b548227",
    "ClientId": "6344c055-0b0e-430a-a1c8-e743d8a3b219",
    "Domain": "your-tenant.onmicrosoft.com",
    "Audience": "api://6344c055-0b0e-430a-a1c8-e743d8a3b219"
  },
  "GraphProvisioning": {
    "TenantId": "ce0aea9c-1d39-404d-a541-3be93b548227",
    "ClientId": "<graph-confidential-client-id>",
    "ClientSecret": "<store-in-user-secrets-or-key-vault>",
    "Scope": "https://graph.microsoft.com/.default",
    "BaseUrl": "https://graph.microsoft.com/v1.0/",
    "DefaultDomain": "your-tenant.onmicrosoft.com"
  },
  "SwaggerOAuth": {
    "ClientId": "6344c055-0b0e-430a-a1c8-e743d8a3b219",
    "UsePkce": true,
    "Scopes": [
      "api://6344c055-0b0e-430a-a1c8-e743d8a3b219/order.read",
      "api://6344c055-0b0e-430a-a1c8-e743d8a3b219/order.write"
    ]
  }
}
```

Do not store secrets in `appsettings.json`. If you later add confidential-client flows, use Secret Manager in development and a secure secret store in production.

## How To Configure Entra

1. Create or reuse an Entra app registration for the APIs.
2. Set the Application ID URI to `api://6344c055-0b0e-430a-a1c8-e743d8a3b219` or update `AzureAd:Audience`.
3. Create delegated scopes:
   - `order.read`
   - `order.write`
4. Create an app role named `Admin`.
5. Create or reuse a confidential client application for Microsoft Graph provisioning.
6. Grant Microsoft Graph application permission `User.ReadWrite.All` to that confidential client and grant admin consent.
7. Configure `GraphProvisioning:ClientId`, `GraphProvisioning:ClientSecret`, and `GraphProvisioning:DefaultDomain`.
8. Grant the API scopes to the client application you will use for testing.
9. Assign the `Admin` app role to test users who need delete access or provisioning access.
10. If you want Swagger UI to complete interactive sign-in, ensure the Swagger client ID is valid for your tenant and supports the configured redirect URI.

Typical Swagger redirect URI:

```text
https://localhost:7101/swagger/oauth2-redirect.html
https://localhost:7201/swagger/oauth2-redirect.html
```

If you prefer, register a separate public client for Swagger UI and place that client ID under `SwaggerOAuth:ClientId`.

## How To Run

1. Open the solution folder:

   ```powershell
   cd C:\Users\HP\Documents\Playground\NCJUAEntraAuthPOC
   ```

2. Restore packages:

   ```powershell
   dotnet restore
   ```

   Configure local Graph secrets for `Auth.Service`:

   ```powershell
   dotnet user-secrets --project .\Auth.Service\Auth.Service.csproj set "GraphProvisioning:ClientId" "<graph-client-id>"
   dotnet user-secrets --project .\Auth.Service\Auth.Service.csproj set "GraphProvisioning:ClientSecret" "<graph-client-secret>"
   ```

3. Run the profile service:

   ```powershell
   dotnet run --project .\Auth.Service\Auth.Service.csproj
   ```

4. Run the order service in a second terminal:

   ```powershell
   dotnet run --project .\Order.Service\Order.Service.csproj
   ```

Swagger URLs:

- `https://localhost:7101/swagger`
- `https://localhost:7201/swagger`

Helper scripts:

- `.\scripts\Get-EntraAccessToken.ps1`
- `.\scripts\Test-ProtectedApiFlow.ps1 -AccessToken "<token>"`

## How To Test With Swagger

1. Start one or both services.
2. Open Swagger UI.
3. Click `Authorize`.
4. Sign in through Microsoft Entra ID.
5. Request the scopes needed for the endpoint you want to test.
6. Call `GET /api/auth/me` to confirm the token is being validated.
7. Call `GET /api/orders` with `order.read`.
8. Call `POST /api/orders` or `PUT /api/orders/{id}` with `order.write`.
9. Call `DELETE /api/orders/{id}` with a user assigned the `Admin` app role.
10. Call `POST /api/users/register` with a user assigned the `Admin` app role.

Sample registration payload:

```json
{
  "userName": "jane.doe",
  "displayName": "Jane Doe",
  "givenName": "Jane",
  "surname": "Doe",
  "email": "jane.doe@contoso.com",
  "temporaryPassword": "TempP@ssw0rd123!",
  "forceChangePasswordNextSignIn": true,
  "accountEnabled": true
}
```

## How To Obtain An Access Token

You can obtain a token from:

- Swagger UI using the configured OAuth2 flow.
- Postman using OAuth 2.0 Authorization Code with PKCE.
- A SPA such as React or Angular using MSAL.
- Azure CLI or a custom confidential client for automated testing.

Example using the included Azure CLI helper:

```powershell
$token = .\scripts\Get-EntraAccessToken.ps1 `
  -Scope "api://6344c055-0b0e-430a-a1c8-e743d8a3b219/order.read"
```

Then validate the token against both services:

```powershell
.\scripts\Test-ProtectedApiFlow.ps1 -AccessToken $token
```

To provision a user after obtaining an admin token:

```powershell
$adminToken = "<entra-admin-access-token>"
$body = @{
  userName = "jane.doe"
  displayName = "Jane Doe"
  givenName = "Jane"
  surname = "Doe"
  email = "jane.doe@contoso.com"
  temporaryPassword = "TempP@ssw0rd123!"
  forceChangePasswordNextSignIn = $true
  accountEnabled = $true
} | ConvertTo-Json

Invoke-RestMethod `
  -Method Post `
  -Uri "https://localhost:7101/api/users/register" `
  -Headers @{ Authorization = "Bearer $adminToken" } `
  -ContentType "application/json" `
  -Body $body
```

Example token request values:

- Authority: `https://login.microsoftonline.com/ce0aea9c-1d39-404d-a541-3be93b548227`
- Scope: `api://6344c055-0b0e-430a-a1c8-e743d8a3b219/order.read`

## How To Call Order.Service

Example request using a bearer token:

```powershell
$token = "<access-token>"

Invoke-RestMethod `
  -Method Get `
  -Uri "https://localhost:7201/api/orders" `
  -Headers @{ Authorization = "Bearer $token" }
```

Example create request:

```powershell
$token = "<access-token>"
$body = @{
  customerName = "Fabrikam"
  productName = "HoloLens"
  quantity = 3
} | ConvertTo-Json

Invoke-RestMethod `
  -Method Post `
  -Uri "https://localhost:7201/api/orders" `
  -Headers @{ Authorization = "Bearer $token" } `
  -ContentType "application/json" `
  -Body $body
```

## Logging

The sample logs:

- authenticated user name
- object ID
- scopes
- roles
- provisioning failures returned by Microsoft Graph
- authentication failures
- authorization failures
- unhandled exceptions

## Key Implementation Notes

- Controller-based APIs only. Minimal APIs are not used.
- Shared authentication registration is centralized in `Shared.Authentication`.
- Both services call:

```csharp
builder.Services.AddSharedAuthentication(builder.Configuration);
builder.Services.AddSharedAuthorization();
```

- `Order.Service` uses an in-memory repository only.
- Nullable reference types are enabled.
- XML documentation generation is enabled.
- All service and controller operations use asynchronous APIs.
