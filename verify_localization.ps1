
$apiUrl = "http://localhost:5000/api/Auth/login"
$loginRequest = @{
    UserName     = "admin@pho24.vn"
    Password     = "password123" # Standard password usually
    MerchantName = "pho24"
    IsRemember   = $false
} | ConvertTo-Json

try {
    Write-Host "Logging in..."
    $response = Invoke-RestMethod -Uri $apiUrl -Method Post -Body $loginRequest -ContentType "application/json"
    $token = $response.AccessToken

    if ($token) {
        Write-Host "✓ Login Successful!"
        $headers = @{
            "Authorization"   = "Bearer $token"
            "Accept-Language" = "ko"
        }
        
        $testUrl = "http://localhost:5000/api/Data/Product/Product%2F4022"
        Write-Host "Testing Korean localization at: $testUrl"
        $testResponse = Invoke-RestMethod -Uri $testUrl -Method Get -Headers $headers
        
        Write-Host "=== API RESPONSE (KO) ===" -ForegroundColor Green
        $testResponse | ConvertTo-Json -Depth 5
        
        if ($testResponse.Name_ko -or $testResponse.Description_ko) {
            Write-Host "✗ FAIL: Suffix fields (Name_ko/Description_ko) are still present!" -ForegroundColor Red
        }
        else {
            Write-Host "✓ SUCCESS: Suffix fields are hidden." -ForegroundColor Green
        }
        
        if ($testResponse.Name -eq "블랙 커피") {
            Write-Host "✓ SUCCESS: Name is correctly mapped to Korean." -ForegroundColor Green
        }
        else {
            Write-Host "✗ WARNING: Name is '$($testResponse.Name)', expected '블랙 커피'" -ForegroundColor Yellow
        }

    }
}
catch {
    Write-Host "Error: $($_.Exception.Message)"
}
