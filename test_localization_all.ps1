
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
        $langs = @("ko", "en", "vi")
        $productId = "Product%2F4022"
        $testUrl = "http://localhost:5000/api/Data/Product/$productId"

        foreach ($l in $langs) {
            Write-Host "`n=== TESTING LANG: $l ===" -ForegroundColor Cyan
            $headers = @{
                "Authorization"   = "Bearer $token"
                "Accept-Language" = $l
            }
            try {
                $res = Invoke-RestMethod -Uri $testUrl -Method Get -Headers $headers
                Write-Host "ID: $($res._id)"
                Write-Host "Name: $($res.Name)"
                Write-Host "Description: $($res.Description)"
            }
            catch {
                Write-Host "Error testing ${l}: $($_.Exception.Message)"
            }
        }
    }
}
catch {
    Write-Host "Login failed: $($_.Exception.Message)"
}
