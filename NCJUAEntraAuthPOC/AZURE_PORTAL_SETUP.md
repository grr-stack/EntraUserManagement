# Azure Portal - Complete Setup Instructions for Graph API

## Navigate to Azure Portal

1. Go to **[portal.azure.com](https://portal.azure.com)**
2. Sign in with your admin account
3. Search for **"App registrations"** in the search bar
4. Click **App registrations**

---

## Create New App Registration for Graph API

1. Click **+ New registration**
2. Fill in the form:
   - **Name**: `Auth Service Graph Provisioning`
   - **Supported account types**: `Accounts in this organizational directory only (Single tenant)`
   - **Redirect URI**: Leave empty (not needed for service account)
3. Click **Register**

📝 **Save these IDs from the overview page:**
- **Application (client) ID** ← Copy this
- **Directory (tenant) ID** ← Should be `ce0aea9c-1d39-404d-a541-3be93b548227`

---

## Grant API Permissions

1. In your newly created app, go to **API permissions** (left sidebar)
2. Click **+ Add a permission**
3. Select **Microsoft Graph**
4. Click **Application permissions** (NOT "Delegated permissions")
5. In the search box, type `User.ReadWrite`
6. Check the box for **User.ReadWrite.All**
7. Click **Add permissions**
8. Back in API permissions, click **Grant admin consent for [Your Org]**
9. Click **Yes** when prompted

**Verify**: You should see `User.ReadWrite.All` with status **"Granted for [Your Org]"** ✅

---

## Create Client Secret

1. Go to **Certificates & secrets** (left sidebar)
2. Click **+ New client secret**
3. In the dialog:
   - **Description**: `Graph API Access`
   - **Expires**: `24 months`
4. Click **Add**
5. **IMMEDIATELY COPY the secret value** (it won't show again!)

📝 **Save this:**
- **Client secret value** ← Copy this before closing

---

## Your Credentials

You now have:
```
Client ID:     [Your Application (client) ID]
Client Secret: [Your new client secret value]
Tenant ID:     ce0aea9c-1d39-404d-a541-3be93b548227
```

---

## Update appsettings.json

1. Open `NCJUAEntraAuthPOC\Auth.Service\appsettings.json` in Visual Studio
2. Find the `GraphProvisioning` section
3. Replace the empty values:

```json
"GraphProvisioning": {
  "TenantId": "ce0aea9c-1d39-404d-a541-3be93b548227",
  "ClientId": "PASTE_YOUR_CLIENT_ID_HERE",
  "ClientSecret": "PASTE_YOUR_CLIENT_SECRET_HERE",
  "Scope": "https://graph.microsoft.com/.default",
  "BaseUrl": "https://graph.microsoft.com/v1.0/",
  "DefaultDomain": "your-tenant.onmicrosoft.com"
}
```

**Example of what it should look like:**
```json
"GraphProvisioning": {
  "TenantId": "ce0aea9c-1d39-404d-a541-3be93b548227",
  "ClientId": "550e8400-e29b-41d4-a716-446655440000",
  "ClientSecret": "eXt8Q~ABC123DEF456GHI789JKL123mno456pqr",
  "Scope": "https://graph.microsoft.com/.default",
  "BaseUrl": "https://graph.microsoft.com/v1.0/",
  "DefaultDomain": "your-tenant.onmicrosoft.com"
}
```

---

## Find Your Tenant Domain

1. In Azure Portal, search for **"Custom domain names"**
2. Look for the domain ending in `.onmicrosoft.com`
3. Copy it and update `DefaultDomain`:

```json
"DefaultDomain": "your-tenant.onmicrosoft.com"
```

---

## Add Admin Role to Your User (for testing)

1. In Azure Portal, go to **Azure AD** → **Users**
2. Click on your user (the one you're logged in with)
3. Click **Directory role** in the left sidebar
4. Click **Add assignments**
5. Select **Cloud application administrator** or **User administrator**
6. Click **Select**
7. Click **Add**

✅ Your user now has Admin role - can call `/api/users/register`

---

## Restart and Test

1. **Save** appsettings.json
2. **Stop** Auth.Service (Visual Studio: Debug → Stop Debugging)
3. **Start** Auth.Service (Visual Studio: Debug → Start Debugging)
4. Open `https://localhost:7101/swagger`
5. Login with OAuth2
6. Try registering "Guru" user

---

## Verify User Created

1. Go to Azure Portal → **Azure AD** → **Users**
2. Search for **"Guru"**
3. You should see:
   - **Display Name**: Guru Sharma
   - **User principal name**: Guru@your-tenant.onmicrosoft.com
   - **Status**: Enabled ✅

---

## Troubleshooting

### "Graph provisioning is not configured"
- Open `appsettings.json`
- Check that `ClientId` and `ClientSecret` are NOT empty
- Restart the application

### "Insufficient privileges"
- Make sure you have the **User administrator** or **Cloud application administrator** role
- Check that you granted admin consent in API permissions

### "Application not found in directory"
- Check the `ClientId` in appsettings.json matches the Graph app (not the main API app)
- The Graph app should be a NEW registration you created

### "Invalid client secret"
- The secret might be expired or wrong
- Create a new client secret in Azure Portal
- Copy the new value to appsettings.json

---

## Done! 🎉

You're ready to:
1. Login via Swagger UI
2. Register users (like "Guru") via `/api/users/register`
3. See them appear in Azure AD Users
