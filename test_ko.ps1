
$apiUrl = "http://localhost:5000/api/Auth/login"
$loginRequest = @{
    UserName     = "admin@pho24.vn"
    Password     = "password123"
    MerchantName = "pho24"
    IsRemember   = $false
} | ConvertTo-Json

try {
    Write-Host "Logging in..."
    $loginResponse = Invoke-RestMethod -Uri $apiUrl -Method Post -Body $loginRequest -ContentType "application/json"
    $token = $loginResponse.AccessToken

    if ($token) {
        $headers = @{
            "Authorization"   = "Bearer $token"
            "Accept-Language" = "ko"
        }
        
        $testUrl = "http://localhost:5000/api/Data/Product/Product%2F4022"
        Write-Host "Testing Korean localization at: $testUrl"
        $testResponse = Invoke-RestMethod -Uri $testUrl -Method Get -Headers $headers
        
        Write-Host "=== API RESPONSE (KO) ===" -ForegroundColor Green
        $testResponse | ConvertTo-Json -Depth 5
    }
}
catch {
    Write-Host "Error: $($_.Exception.Message)"
}
