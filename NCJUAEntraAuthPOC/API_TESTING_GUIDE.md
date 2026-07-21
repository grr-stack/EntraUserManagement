# API Testing Guide - Entra Auth POC

## Quick Start

### 1. Get Your JWT Token

1. Start both services (Auth.Service on 7101, Order.Service on 7201)
2. Open `https://localhost:7101/swagger` in your browser
3. Click the **"Authorize"** button
4. Login with your Azure AD credentials using OAuth2 PKCE
5. You'll be redirected back with an authorized session

### 2. Extract Your JWT Token

**Option A - From Browser Console:**
```javascript
// Open browser DevTools (F12) → Console tab
// Look for your token in localStorage or sessionStorage
localStorage.getItem('token')
```

**Option B - Use the /api/auth/me endpoint:**
```bash
# This returns your token details and user info
GET https://localhost:7101/api/auth/me
```

### 3. Copy Token to .http Files

Update both `.http` files with your JWT token:
- `NCJUAEntraAuthPOC\Auth.Service\requests.http`
- `NCJUAEntraAuthPOC\Order.Service\orders-requests.http`

Replace:
```
@jwtToken = Bearer YOUR_JWT_TOKEN_HERE
```

With:
```
@jwtToken = Bearer eyJhbGciOiJSUzI1NiIsImtpZCI6IjIx... (your actual token)
```

---

## API Endpoints

### Auth Service (https://localhost:7101)

| Method | Endpoint | Auth Required | Role Required | Description |
|--------|----------|---------------|---------------|-------------|
| GET | `/api/auth/health` | ❌ No | - | Health check |
| GET | `/api/auth/me` | ✅ Yes | - | Get current user profile |
| GET | `/api/auth/claims` | ✅ Yes | - | Get all JWT claims |
| POST | `/api/users/register` | ✅ Yes | Admin | Register new user in Entra ID |

### Order Service (https://localhost:7201)

| Method | Endpoint | Auth Required | Scope Required | Description |
|--------|----------|---------------|----------------|-------------|
| GET | `/api/orders` | ✅ Yes | order.read | Get all orders |
| GET | `/api/orders/{id}` | ✅ Yes | order.read | Get order by ID |
| POST | `/api/orders` | ✅ Yes | order.write | Create new order |
| PUT | `/api/orders/{id}` | ✅ Yes | order.write | Update order |
| DELETE | `/api/orders/{id}` | ✅ Yes | order.write | Delete order |

---

## Testing Workflow

### Step 1: Verify Service Health
```http
GET https://localhost:7101/api/auth/health
```
✅ Expected: `{"status":"OK"}`

### Step 2: Get Your User Profile
```http
GET https://localhost:7101/api/auth/me
Authorization: Bearer YOUR_JWT_TOKEN
```
✅ Expected: User details with roles, scopes, and claims

### Step 3: Register a New User (Admin Only)
```http
POST https://localhost:7101/api/users/register
Authorization: Bearer YOUR_JWT_TOKEN
Content-Type: application/json

{
  "userName": "john.doe",
  "displayName": "John Doe",
  "givenName": "John",
  "surname": "Doe",
  "email": "john.doe@example.com"
}
```
✅ Expected: `201 Created` with user details from Entra ID
✅ The user will appear in your Azure AD tenant

### Step 4: Create Orders
```http
POST https://localhost:7201/api/orders
Authorization: Bearer YOUR_JWT_TOKEN
Content-Type: application/json

{
  "productName": "Laptop",
  "quantity": 2,
  "unitPrice": 999.99,
  "orderDate": "2026-06-30T10:00:00Z"
}
```
✅ Expected: `201 Created` with order details

### Step 5: Read Orders
```http
GET https://localhost:7201/api/orders
Authorization: Bearer YOUR_JWT_TOKEN
```
✅ Expected: `200 OK` with array of orders

### Step 6: Update Orders
```http
PUT https://localhost:7201/api/orders/{id}
Authorization: Bearer YOUR_JWT_TOKEN
Content-Type: application/json

{
  "productName": "Updated Product",
  "quantity": 5,
  "unitPrice": 1299.99,
  "orderDate": "2026-06-30T10:00:00Z"
}
```
✅ Expected: `200 OK` with updated order

### Step 7: Delete Orders
```http
DELETE https://localhost:7201/api/orders/{id}
Authorization: Bearer YOUR_JWT_TOKEN
```
✅ Expected: `204 No Content`

---

## Understanding Token Scopes

Your JWT token contains scopes that determine what you can do:

| Scope | Service | Allows |
|-------|---------|--------|
| `order.read` | Order.Service | Read orders (GET) |
| `order.write` | Order.Service | Create/Update/Delete orders (POST, PUT, DELETE) |

**Important:** Only endpoints with matching scopes in your token will succeed. Others return `403 Forbidden`.

---

## Understanding JWT Claims

Your JWT contains these important claims:

| Claim | Example | Purpose |
|-------|---------|---------|
| `oid` | `550e8400-e29b-41d4-a716-446655440000` | Azure AD Object ID (unique user identifier) |
| `preferred_username` | `user@yourdomain.com` | Email address |
| `name` | `John Doe` | Display name |
| `roles` | `["Admin"]` | App roles (for authorization) |
| `scp` | `order.read order.write` | Delegated scopes (for authorization) |
| `aud` | `api://6344c055-0b0e-430a-a1c8-e743d8a3b219` | Audience (API ID) |
| `iss` | `https://login.microsoftonline.com/.../v2.0` | Issuer (Azure AD) |

---

## Error Responses

| Status | Error | Meaning | Solution |
|--------|-------|---------|----------|
| 401 | `Unauthorized` | Missing or invalid token | Use valid JWT token |
| 403 | `Forbidden` | Insufficient permissions/scopes | Login with user having Admin role or required scopes |
| 404 | `Not Found` | Resource doesn't exist | Verify ID exists |
| 400 | `Bad Request` | Invalid request body | Check request format and required fields |
| 500 | `Internal Server Error` | Server error | Check application logs |

---

## .http File Usage

### Visual Studio Code
1. Install "REST Client" extension by Huachao Zheng
2. Open any `.http` file
3. Click "Send Request" above each endpoint

### Visual Studio (Built-in)
1. Open any `.http` file
2. Click the play button (▶) or right-click → "Send Request"
3. Response appears in new window

### Command Line (curl)
```bash
curl -X GET "https://localhost:7101/api/auth/health" \
  -H "Accept: application/json"

curl -X GET "https://localhost:7101/api/auth/me" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Accept: application/json"
```

---

## Troubleshooting

### Issue: "AADSTS something" error on OAuth2 login
**Solution:** Check Azure Portal app registration settings (redirect URI, implicit grant, etc.)

### Issue: "401 Unauthorized" on all endpoints
**Solution:** Your JWT token is missing or invalid. Get a new one by logging in again.

### Issue: "403 Forbidden" on order endpoints
**Solution:** Your token doesn't have the required scope. Login with a user that has order.read/order.write scopes configured.

### Issue: Register endpoint fails with 403
**Solution:** You don't have the "Admin" role. Only admin users can register new users.

### Issue: Created user doesn't appear in Entra ID
**Solution:** Check `appsettings.json` GraphProvisioning section:
- ClientId and ClientSecret must be valid
- Service account must have User Administrator permissions in Azure AD
- Check server logs for Graph API errors

---

## Files Created

- `NCJUAEntraAuthPOC\Auth.Service\requests.http` - Auth service endpoints
- `NCJUAEntraAuthPOC\Order.Service\orders-requests.http` - Order service endpoints
- `API_TESTING_GUIDE.md` - This file

---

## Next Steps

1. ✅ Run both services
2. ✅ Get JWT token via OAuth2 login
3. ✅ Update `.http` files with your token
4. ✅ Test endpoints in order (health → me → register → orders)
5. ✅ Verify new user appears in Azure AD portal
6. ✅ Check server logs for any issues
