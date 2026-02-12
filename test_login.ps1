# Test Login API and Get JWT Token
# Usage: .\test_login.ps1

$apiUrl = "http://localhost:5271/api/Auth/login"

# UPDATE THESE CREDENTIALS
$loginRequest = @{
    UserName = "your-email@example.com"      # Change this
    Password = "your-password"                # Change this
    MerchantName = "your-merchant-name"      # Change this
    IsRemember = $false
} | ConvertTo-Json

Write-Host "=== Testing Login API ===" -ForegroundColor Cyan
Write-Host "API URL: $apiUrl"
Write-Host ""

try {
    $response = Invoke-RestMethod -Uri $apiUrl -Method Post -Body $loginRequest -ContentType "application/json"
    
    Write-Host "✓ Login Successful!" -ForegroundColor Green
    Write-Host ""
    
    if ($response.AccessToken) {
        Write-Host "=== ACCESS TOKEN ===" -ForegroundColor Yellow
        Write-Host $response.AccessToken
        Write-Host ""
        
        Write-Host "=== FULL RESPONSE ===" -ForegroundColor Cyan
        $response | ConvertTo-Json -Depth 10
        Write-Host ""
        
        Write-Host "=== HOW TO USE ===" -ForegroundColor Green
        Write-Host "Add this header to your requests:"
        Write-Host "Authorization: Bearer $($response.AccessToken)" -ForegroundColor Yellow
        Write-Host ""
        
        # Test the token with a Data API call
        Write-Host "=== Testing Token with Data API ===" -ForegroundColor Cyan
        $headers = @{
            "Authorization" = "Bearer $($response.AccessToken)"
        }
        
        try {
            $testUrl = "http://localhost:5271/api/Data/Customer?limit=1"
            Write-Host "Testing: $testUrl"
            $testResponse = Invoke-RestMethod -Uri $testUrl -Method Get -Headers $headers
            Write-Host "✓ Token works! Sample response:" -ForegroundColor Green
            $testResponse | ConvertTo-Json -Depth 3
        }
        catch {
            Write-Host "Note: Token retrieved but test API call failed" -ForegroundColor Yellow
            Write-Host "Error: $($_.Exception.Message)"
        }
    }
    else {
        Write-Host "Response:"
        $response | ConvertTo-Json -Depth 10
    }
}
catch {
    Write-Host "✗ Login Failed!" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)"
    Write-Host ""
    
    if ($_.ErrorDetails.Message) {
        Write-Host "Response:" -ForegroundColor Yellow
        $_.ErrorDetails.Message | ConvertFrom-Json | ConvertTo-Json -Depth 10
    }
    
    Write-Host ""
    Write-Host "Make sure:" -ForegroundColor Yellow
    Write-Host "1. The API is running (dotnet run --project services/Zap.Identity.Api)"
    Write-Host "2. The API is accessible at http://localhost:5271"
    Write-Host "3. MongoDB is running and accessible"
    Write-Host "4. You have updated the credentials in this script"
}
