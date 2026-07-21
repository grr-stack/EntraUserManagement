param(
    [Parameter(Mandatory = $true)]
    [string]$AccessToken,
    [string]$AuthServiceBaseUrl = "https://localhost:7101",
    [string]$OrderServiceBaseUrl = "https://localhost:7201"
)

$headers = @{
    Authorization = "Bearer $AccessToken"
}

Write-Host "Calling Auth.Service /api/auth/me..." -ForegroundColor Cyan
Invoke-RestMethod `
    -Method Get `
    -Uri "$AuthServiceBaseUrl/api/auth/me" `
    -Headers $headers

Write-Host "Calling Order.Service /api/orders..." -ForegroundColor Cyan
Invoke-RestMethod `
    -Method Get `
    -Uri "$OrderServiceBaseUrl/api/orders" `
    -Headers $headers
