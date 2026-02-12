# Quick Login Test
$apiUrl = "http://localhost:5271/api/Auth/login"

$loginRequest = @{
    UserName     = "admin@pho24.vn"
    Password     = "your-password-here"  # UPDATE THIS
    MerchantName = "pho24"           # UPDATE THIS IF NEEDED
    IsRemember   = $true
} | ConvertTo-Json

Write-Host "=== Attempting Login ===" -ForegroundColor Cyan
Write-Host "Email: admin@pho24.vn"
Write-Host ""

try {
    $response = Invoke-RestMethod -Uri $apiUrl -Method Post -Body $loginRequest -ContentType "application/json"
    
    Write-Host "✓ Login Successful!" -ForegroundColor Green
    Write-Host ""
    Write-Host "=== NEW ACCESS TOKEN ===" -ForegroundColor Yellow
    Write-Host $response.AccessToken
    Write-Host ""
    Write-Host "Copy this token and use it in your requests!" -ForegroundColor Green
    Write-Host ""
    
    # Save to clipboard if available
    try {
        $response.AccessToken | Set-Clipboard
        Write-Host "✓ Token copied to clipboard!" -ForegroundColor Cyan
    }
    catch {
        # Clipboard not available
    }
}
catch {
    Write-Host "✗ Login Failed!" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)"
    
    if ($_.ErrorDetails.Message) {
        Write-Host ""
        Write-Host "Response:" -ForegroundColor Yellow
        $_.ErrorDetails.Message | ConvertFrom-Json | ConvertTo-Json -Depth 10
    }
    
    Write-Host ""
    Write-Host "Make sure:" -ForegroundColor Yellow
    Write-Host "1. The API is running"
    Write-Host "2. The password is correct"
    Write-Host "3. The merchant name is correct"
}
