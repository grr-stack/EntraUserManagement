param(
    [string]$TenantId = "ce0aea9c-1d39-404d-a541-3be93b548227",
    [string]$Scope = "api://6344c055-0b0e-430a-a1c8-e743d8a3b219/order.read"
)

$az = Get-Command az -ErrorAction SilentlyContinue
if (-not $az)
{
    throw "Azure CLI is required. Install Azure CLI and run this script again."
}

Write-Host "Signing in to tenant $TenantId..." -ForegroundColor Cyan
az login --tenant $TenantId | Out-Null

Write-Host "Requesting access token for scope $Scope..." -ForegroundColor Cyan
$token = az account get-access-token --scope $Scope --query accessToken -o tsv

if ([string]::IsNullOrWhiteSpace($token))
{
    throw "Azure CLI did not return an access token."
}

$token
