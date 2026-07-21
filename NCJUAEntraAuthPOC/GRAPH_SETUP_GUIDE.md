# Setting Up Graph API Credentials for User Provisioning

## Problem
Users aren't being created in Entra ID because the Graph API credentials are missing.

## Solution: Create a Service Principal for Graph API

### Step 1: Create a New App Registration in Azure Portal

1. Go to **[Azure Portal](https://portal.azure.com)**
2. Navigate to **Azure AD** → **App registrations**
3. Click **+ New registration**
4. Fill in:
   - **Name**: `Auth Service Graph Provisioning` (or similar)
   - **Supported account types**: `Accounts in this organizational directory only`
   - **Redirect URI**: Leave blank (not needed for service account)
5. Click **Register**

### Step 2: Grant Permissions to Graph API

1. In the app you just created, go to **API permissions**
2. Click **+ Add a permission**
3. Select **Microsoft Graph**
4. Select **Application permissions** (NOT delegated)
5. Search for and add these permissions:
   - `User.ReadWrite.All` - Create and modify users
   - `Directory.ReadWrite.All` - Read/write directory
6. Click **Grant admin consent for your organization** (Important!)

### Step 3: Create a Client Secret

1. Go to **Certificates & secrets**
2. Click **+ New client secret**
3. Description: `Graph API Access`
4. Expires: `24 months` (or your preference)
5. Click **Add**
6. **COPY the secret value** (you won't see it again!)

### Step 4: Get Your Credentials

You now have:
- **Client ID**: From the app registration overview page
- **Client Secret**: From the secret you just created
- **Tenant ID**: `ce0aea9c-1d39-404d-a541-3be93b548227` (already configured)

### Step 5: Update appsettings.json

Update `NCJUAEntraAuthPOC\Auth.Service\appsettings.json`:

```json
"GraphProvisioning": {
  "TenantId": "ce0aea9c-1d39-404d-a541-3be93b548227",
  "ClientId": "YOUR_NEW_APP_CLIENT_ID",
  "ClientSecret": "YOUR_NEW_APP_CLIENT_SECRET",
  "Scope": "https://graph.microsoft.com/.default",
  "BaseUrl": "https://graph.microsoft.com/v1.0/",
  "DefaultDomain": "your-tenant.onmicrosoft.com"
}
```

Replace:
- `YOUR_NEW_APP_CLIENT_ID` with the Client ID from Step 3
- `YOUR_NEW_APP_CLIENT_SECRET` with the Client Secret from Step 3

### Step 6: Update appsettings.Development.json (Optional)

For development, you can also add to `appsettings.Development.json`:

```json
{
  "GraphProvisioning": {
	"ClientId": "YOUR_NEW_APP_CLIENT_ID",
	"ClientSecret": "YOUR_NEW_APP_CLIENT_SECRET",
	"DefaultDomain": "your-development-tenant.onmicrosoft.com"
  }
}
```

### Step 7: Verify Domain Configuration

Make sure your domain is correct:
```json
"DefaultDomain": "your-tenant.onmicrosoft.com"
```

Find your tenant domain:
1. Go to Azure Portal → **Azure AD** → **Custom domain names**
2. Copy the `.onmicrosoft.com` domain

### Step 8: Restart and Test

1. Save the changes
2. Stop and restart the Auth.Service application
3. Try registering a user again

---

## Testing the Setup

Use the `/api/users/register` endpoint with this payload:

```json
{
  "userName": "Guru",
  "displayName": "Guru Sharma",
  "givenName": "Guru",
  "surname": "Sharma",
  "email": "guru@your-tenant.onmicrosoft.com"
}
```

### Expected Success Response (201 Created):
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "userPrincipalName": "Guru@your-tenant.onmicrosoft.com",
  "displayName": "Guru Sharma",
  "email": "guru@your-tenant.onmicrosoft.com",
  "accountEnabled": true
}
```

### Verify in Azure Portal
1. Go to **Azure AD** → **Users**
2. Search for "Guru"
3. User should appear in the list

---

## Troubleshooting

### Error: "Graph provisioning is not configured"
- ✅ Check that `ClientId` and `ClientSecret` are NOT empty in `appsettings.json`
- ✅ Make sure you restarted the application after changes

### Error: "Insufficient privileges to complete the operation"
- ✅ Make sure you granted `User.ReadWrite.All` permission
- ✅ Make sure you clicked "Grant admin consent"

### Error: "AADSTS700016: Application not found in directory"
- ✅ Use the correct `ClientId` from the app registration
- ✅ Make sure it's the NEW app (for Graph provisioning), not the original API app

### Error: "AADSTS7000218: The request body must contain the following parameter: 'client_assertion' or 'client_secret'"
- ✅ Your `ClientSecret` might be expired or wrong
- ✅ Create a new client secret if needed

---

## Important Security Note

⚠️ **Never commit credentials to Git!**

For production, use:
- **Azure Key Vault** to store secrets
- **Environment variables** for local development
- **Managed Identities** if running in Azure

Example with environment variable:
```csharp
"ClientSecret": "${GRAPH_CLIENT_SECRET}"
```

Then set in PowerShell:
```powershell
$env:GRAPH_CLIENT_SECRET = "your-secret-value"
```
