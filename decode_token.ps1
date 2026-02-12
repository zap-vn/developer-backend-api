# Decode JWT Token
$token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VyR3VpZCI6IkN1c3RvbWVyLzEiLCJFbXBsb3llZUd1aWQiOiJDdXN0b21lci8xIiwiUm9sZU5hbWUiOiJPd25lciAoU3VwZXIgQWRtaW4pIiwiUm9sZVBlcm1pc3Npb25faWQiOiI2NTdhYjE1ZDU0ZjE3MzMzZjNkODljNjUiLCJMYW5ndWFnZSI6InZpIiwic3ViIjoiMSIsImVtYWlsIjoiYWRtaW5AcGhvMjQudm4iLCJqdGkiOiIwNmQ4Mzg1NS05ODUzLTRjMWUtOGJjMy00MGM1M2I2MTE4ZTMiLCJpYXQiOjE3NzA4NjUxMDIsImV4cCI6MTc3MDk1MTUwMiwiaXNzIjoiaHR0cHM6Ly9kZXYtY3JtLW1lcmNoYW50LWFwaS5kaWFkaWVtLnZuIiwiYXVkIjoidHJhbnZ1b25nIE1KIn0.x4miSnKPXMNEoa3AjpAf46ye85l3pBS2SsNVj8AuZ3A"

Write-Host "=== JWT Token Decoder ===" -ForegroundColor Cyan
Write-Host ""

# Split token
$parts = $token.Split('.')
if ($parts.Length -ne 3) {
    Write-Host "Invalid JWT token format!" -ForegroundColor Red
    exit
}

# Decode header
$headerJson = [System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String($parts[0] + "=="))
$header = $headerJson | ConvertFrom-Json

# Decode payload
$payloadPadded = $parts[1]
while ($payloadPadded.Length % 4 -ne 0) {
    $payloadPadded += "="
}
$payloadJson = [System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String($payloadPadded))
$payload = $payloadJson | ConvertFrom-Json

Write-Host "=== HEADER ===" -ForegroundColor Yellow
$header | ConvertTo-Json
Write-Host ""

Write-Host "=== PAYLOAD (Claims) ===" -ForegroundColor Yellow
$payload | ConvertTo-Json
Write-Host ""

# Check expiration
$currentTime = [DateTimeOffset]::UtcNow.ToUnixTimeSeconds()
$expirationTime = $payload.exp
$issuedTime = $payload.iat

$expirationDate = [DateTimeOffset]::FromUnixTimeSeconds($expirationTime).LocalDateTime
$issuedDate = [DateTimeOffset]::FromUnixTimeSeconds($issuedTime).LocalDateTime

Write-Host "=== TOKEN VALIDITY ===" -ForegroundColor Cyan
Write-Host "Issued At:  $issuedDate"
Write-Host "Expires At: $expirationDate"
Write-Host "Current:    $([DateTime]::Now)"
Write-Host ""

if ($currentTime -gt $expirationTime) {
    Write-Host "⚠ TOKEN EXPIRED!" -ForegroundColor Red
    $expiredSince = [TimeSpan]::FromSeconds($currentTime - $expirationTime)
    Write-Host "Expired $($expiredSince.TotalHours.ToString('N2')) hours ago"
}
else {
    Write-Host "✓ Token is still valid" -ForegroundColor Green
    $validFor = [TimeSpan]::FromSeconds($expirationTime - $currentTime)
    Write-Host "Valid for $($validFor.TotalHours.ToString('N2')) more hours"
}
Write-Host ""

Write-Host "=== KEY CLAIMS ===" -ForegroundColor Cyan
Write-Host "User GUID:  $($payload.UserGuid)"
Write-Host "Email:      $($payload.email)"
Write-Host "Role:       $($payload.RoleName)"
Write-Host "Language:   $($payload.Language)"
Write-Host ""

Write-Host "=== ISSUER/AUDIENCE ===" -ForegroundColor Cyan
Write-Host "Issuer:   $($payload.iss)"
Write-Host "Audience: $($payload.aud)"
Write-Host ""

# Test the token
Write-Host "=== TESTING TOKEN ===" -ForegroundColor Yellow
Write-Host "Testing against: http://localhost:5271/api/Data/Customer?limit=1"
Write-Host ""

try {
    $headers = @{
        "Authorization" = "Bearer $token"
    }
    
    $testResponse = Invoke-RestMethod -Uri "http://localhost:5271/api/Data/Customer?limit=1" -Method Get -Headers $headers -ErrorAction Stop
    
    Write-Host "✓ TOKEN WORKS!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Sample response:"
    $testResponse | ConvertTo-Json -Depth 3
    Write-Host ""
    Write-Host "=== SUCCESS! ===" -ForegroundColor Green
    Write-Host "You can now use this token with your API requests."
    Write-Host ""
    Write-Host "Authorization: Bearer $token"
}
catch {
    $statusCode = $_.Exception.Response.StatusCode.value__
    Write-Host "✗ Token test failed with status code: $statusCode" -ForegroundColor Red
    
    if ($statusCode -eq 401) {
        Write-Host ""
        Write-Host "Possible reasons:" -ForegroundColor Yellow
        Write-Host "1. Token signature doesn't match the API's JWT secret"
        Write-Host "2. Issuer/Audience validation is failing"
        Write-Host "3. Token has expired"
        Write-Host ""
        Write-Host "Your token has:" -ForegroundColor Cyan
        Write-Host "  Issuer: $($payload.iss)"
        Write-Host "  Audience: $($payload.aud)"
        Write-Host ""
        Write-Host "Check your appsettings.json to ensure ValidateIssuer and ValidateAudience are set correctly."
    }
    else {
        Write-Host "Error details: $($_.Exception.Message)"
    }
    
    if ($_.ErrorDetails.Message) {
        Write-Host ""
        Write-Host "Response:" -ForegroundColor Yellow
        $_.ErrorDetails.Message
    }
}
