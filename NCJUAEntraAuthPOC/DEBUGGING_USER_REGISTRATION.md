# Debugging User Registration - Step-by-Step

## Prerequisites Checklist

Before testing, make sure you have:

- [ ] **Graph API App Registration Created** (separate from your main API app)
- [ ] **Client ID** for the Graph app
- [ ] **Client Secret** for the Graph app
- [ ] **User.ReadWrite.All** permission granted with admin consent
- [ ] **appsettings.json** updated with Graph credentials
- [ ] **Auth.Service restarted** after credential updates
- [ ] **JWT Token** obtained from OAuth2 login
- [ ] **Admin role** - your user must have "Admin" role to call register endpoint

---

## Step-by-Step Testing

### Step 1: Check Service Health

```http
GET https://localhost:7101/api/auth/health
```

✅ **Expected**: `{"status":"OK"}`

If this fails, the service isn't running.

---

### Step 2: Verify Your User Profile

```http
GET https://localhost:7101/api/auth/me
Authorization: Bearer YOUR_JWT_TOKEN
```

✅ **Expected**: Your user profile with roles and scopes

Look for:
```json
{
  "roles": ["Admin"],
  "scopes": ["order.read", "order.write"],
  ...
}
```

⚠️ **If you don't see "Admin" role**: You won't be able to call the register endpoint. You need to add yourself to the Admin role in Azure AD.

---

### Step 3: Test Register Endpoint - Register "Guru"

```http
POST https://localhost:7101/api/users/register
Authorization: Bearer YOUR_JWT_TOKEN
Content-Type: application/json

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

### Possible Responses:

#### ✅ Success (201 Created)
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "userPrincipalName": "Guru@your-tenant.onmicrosoft.com",
  "displayName": "Guru Sharma",
  "email": "guru@your-tenant.onmicrosoft.com",
  "accountEnabled": true
}
```
👉 **Next**: Check Azure Portal → Users → Search "Guru"

---

#### ❌ Error 401 Unauthorized
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Unauthorized",
  "status": 401
}
```
**Cause**: Missing or invalid JWT token  
**Fix**: 
1. Get new token from Swagger UI
2. Update the `@jwtToken` variable in the .http file

---

#### ❌ Error 403 Forbidden
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.3",
  "title": "Forbidden",
  "status": 403
}
```
**Cause**: You don't have Admin role  
**Fix**:
1. Go to Azure Portal → Users
2. Find your user
3. Click → **Azure AD roles**
4. Add **Cloud application administrator** or **User administrator** role

---

#### ❌ Error 502 Bad Gateway (Graph API Error)
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.6.1",
  "title": "Bad Gateway",
  "status": 502,
  "detail": "Microsoft Graph rejected the user registration request.",
  "errors": [
	"error": "AADSTS...",
	"error_description": "..."
  ]
}
```

**Common Graph API Errors:**

| Error | Cause | Fix |
|-------|-------|-----|
| `AADSTS700016: Application not found in directory` | Wrong Client ID | Verify Client ID matches the Graph app registration |
| `AADSTS7000218: client_secret invalid` | Wrong or expired secret | Create new client secret in Azure Portal |
| `AADSTS65001: User or admin has not consented` | Permissions not granted | Go to Graph app → API permissions → Grant admin consent |
| `Authorization_RequestDenied` | Missing permission | Add `User.ReadWrite.All` to Graph app |
| `Request_BadRequest` | Invalid request format | Check email format, password requirements |

---

#### ❌ Error 503 Service Unavailable
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.6.4",
  "title": "Service Unavailable",
  "status": 503,
  "detail": "Graph provisioning is not configured. Set GraphProvisioning:ClientId and GraphProvisioning:ClientSecret."
}
```
**Cause**: `appsettings.json` has empty GraphProvisioning credentials  
**Fix**:
1. Create Graph app registration in Azure Portal
2. Update `appsettings.json`:
```json
"GraphProvisioning": {
  "ClientId": "YOUR_CLIENT_ID",
  "ClientSecret": "YOUR_CLIENT_SECRET"
}
```
3. Restart Auth.Service

---

#### ❌ Error 409 Conflict
```json
{
  "error": "Request_BadRequest",
  "error_description": "Another object with the same value for property userPrincipalName already exists."
}
```
**Cause**: User already exists  
**Fix**: Use a different username or email

---

### Step 4: Verify User in Azure Portal

After successful registration:

1. Go to [Azure Portal](https://portal.azure.com)
2. Navigate to **Azure AD** → **Users**
3. Search for "Guru"
4. You should see:
   - **Name**: Guru Sharma
   - **User principal name**: Guru@your-tenant.onmicrosoft.com
   - **User type**: Member
   - **Status**: Enabled

---

## Logging & Debugging

### Check Application Logs

If you're running locally in Visual Studio:

1. Open **View** → **Output** (or press Ctrl+Alt+O)
2. Select **Auth.Service** from the dropdown
3. Look for errors related to Graph API

Key log patterns:
```
[error] Microsoft Graph user creation failed. Status: 401. Body: ...
[error] Failed to acquire Microsoft Graph token. Status: 401. Body: ...
```

---

## Common Issues Checklist

- [ ] Do you have a **separate Graph app registration** (not the main API app)?
- [ ] Does the Graph app have **User.ReadWrite.All** permission?
- [ ] Did you click **Grant admin consent**?
- [ ] Is the **Client Secret** correct and not expired?
- [ ] Is the **Client ID** correct?
- [ ] Have you **restarted** the Auth.Service after updating credentials?
- [ ] Do you have the **Admin role** in your JWT?
- [ ] Did you include the **temporaryPassword** field in the request?
- [ ] Is your **email address** valid format?

---

## Quick Test Script

Run this sequence in your `.http` file:

1. ✅ Health check
2. ✅ Get /me
3. ✅ Register Guru
4. ✅ Check Azure Portal for Guru user

---

## Success!

When you see "Guru" appear in Azure AD Users list, the setup is complete! 🎉

The user can now:
- Login to the application via OAuth2
- Get their profile via `/api/auth/me`
- Access protected resources based on roles/scopes
