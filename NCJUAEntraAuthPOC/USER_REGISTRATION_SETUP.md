# User Registration Setup - Summary

## The Problem
You tried to register a user but they don't appear in Entra ID.

## The Root Cause
The `GraphProvisioning` credentials in `appsettings.json` are empty:
```json
"GraphProvisioning": {
  "ClientId": "",          // ❌ EMPTY!
  "ClientSecret": ""       // ❌ EMPTY!
}
```

## The Solution
You need to create a separate app registration in Azure AD for Graph API access.

---

## Quick Setup (5 minutes)

### 1️⃣ Create Graph App in Azure Portal

1. Go to [portal.azure.com](https://portal.azure.com)
2. Search for **"App registrations"**
3. Click **+ New registration**
4. Name: `Auth Service Graph Provisioning`
5. Click **Register**
6. **Copy the Client ID from the overview page**

### 2️⃣ Grant Permissions

1. Click **API permissions** on the left
2. Click **+ Add a permission**
3. Select **Microsoft Graph**
4. Click **Application permissions**
5. Search for `User.ReadWrite` and check **User.ReadWrite.All**
6. Click **Add permissions**
7. Click **Grant admin consent for [Your Org]**
8. Click **Yes**

### 3️⃣ Create Secret

1. Click **Certificates & secrets** on the left
2. Click **+ New client secret**
3. **Expires**: `24 months`
4. Click **Add**
5. **COPY the secret value immediately** ⚠️

### 4️⃣ Update appsettings.json

Open `NCJUAEntraAuthPOC\Auth.Service\appsettings.json` and replace:

```json
"GraphProvisioning": {
  "TenantId": "ce0aea9c-1d39-404d-a541-3be93b548227",
  "ClientId": "YOUR_CLIENT_ID_FROM_STEP_1",
  "ClientSecret": "YOUR_SECRET_FROM_STEP_3",
  "Scope": "https://graph.microsoft.com/.default",
  "BaseUrl": "https://graph.microsoft.com/v1.0/",
  "DefaultDomain": "your-tenant.onmicrosoft.com"
}
```

### 5️⃣ Restart Application

1. Stop Auth.Service
2. Start Auth.Service
3. Open `https://localhost:7101/swagger`

### 6️⃣ Register Guru User

1. Click **Authorize** and login
2. Try the register endpoint with this payload:

```json
{
  "userName": "Guru",
  "displayName": "Guru Sharma",
  "givenName": "Guru",
  "surname": "Sharma",
  "email": "guru@your-tenant.onmicrosoft.com",
  "temporaryPassword": "GuruPassword@123456",
  "forceChangePasswordNextSignIn": true,
  "accountEnabled": true
}
```

### 7️⃣ Verify in Azure Portal

1. Go to [portal.azure.com](https://portal.azure.com)
2. Click **Azure AD** → **Users**
3. Search for **"Guru"**
4. ✅ You should see the Guru user!

---

## Documentation Files Created

| File | Purpose |
|------|---------|
| `AZURE_PORTAL_SETUP.md` | Step-by-step Azure Portal instructions |
| `GRAPH_SETUP_GUIDE.md` | Detailed Graph API configuration |
| `DEBUGGING_USER_REGISTRATION.md` | Troubleshooting & error reference |
| `requests.http` | HTTP requests for testing (with Guru example) |
| `API_TESTING_GUIDE.md` | Complete API reference |

---

## Common Issues

| Issue | Solution |
|-------|----------|
| "Graph provisioning is not configured" | Empty ClientId/Secret in appsettings.json |
| "Insufficient privileges" | Add User.ReadWrite.All permission with admin consent |
| "Application not found" | Wrong ClientId (use the Graph app, not the API app) |
| "User already exists" | Use different username/email |
| "403 Forbidden" on register | Your user doesn't have Admin role |

---

## Important Notes

⚠️ **Never commit secrets to Git!**
- Use Azure Key Vault for production
- Use environment variables for local dev
- `.gitignore` should exclude `appsettings.Development.json`

---

## Testing Checklist

- [ ] Created Graph app registration in Azure Portal
- [ ] Copied Client ID and Client Secret
- [ ] Updated appsettings.json with credentials
- [ ] Restarted Auth.Service
- [ ] Added Admin role to your user
- [ ] Got JWT token from Swagger UI
- [ ] Called `/api/users/register` with Guru payload
- [ ] See "Guru" in Azure Portal Users list ✅

---

## Next Steps

Once Guru user is created, you can:

1. **Login as Guru** in the application
2. **Test OAuth2 flow** with the new user
3. **Assign roles/scopes** to Guru via Azure Portal
4. **Test authorization policies** (Admin only, order.read, order.write)

---

Need help? Check the troubleshooting section in `DEBUGGING_USER_REGISTRATION.md`
