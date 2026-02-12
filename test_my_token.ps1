# Test the user's existing token
$token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VyR3VpZCI6IkN1c3RvbWVyLzEiLCJFbXBsb3llZUd1aWQiOiJDdXN0b21lci8xIiwiUm9sZU5hbWUiOiJPd25lciAoU3VwZXIgQWRtaW4pIiwiUm9sZVBlcm1pc3Npb25faWQiOiI2NTdhYjE1ZDU0ZjE3MzMzZjNkODljNjUiLCJMYW5ndWFnZSI6InZpIiwic3ViIjoiMSIsImVtYWlsIjoiYWRtaW5AcGhvMjQudm4iLCJqdGkiOiIwNmQ4Mzg1NS05ODUzLTRjMWUtOGJjMy00MGM1M2I2MTE4ZTMiLCJpYXQiOjE3NzA4NjUxMDIsImV4cCI6MTc3MDk1MTUwMiwiaXNzIjoiaHR0cHM6Ly9kZXYtY3JtLW1lcmNoYW50LWFwaS5kaWFkaWVtLnZuIiwiYXVkIjoidHJhbnZ1b25nIE1KIn0.x4miSnKPXMNEoa3AjpAf46ye85l3pBS2SsNVj8AuZ3A"

Write-Host "=== Testing Your Token ===" -ForegroundColor Cyan
Write-Host ""

$headers = @{
    "Authorization" = "Bearer $token"
}

Write-Host "Testing endpoint: http://localhost:5271/api/Data/Customer?limit=1" -ForegroundColor Yellow
Write-Host ""

try {
    $response = Invoke-RestMethod -Uri "http://localhost:5271/api/Data/Customer?limit=1" -Method Get -Headers $headers -ErrorAction Stop
    
    Write-Host "✓✓✓ SUCCESS! Your token now works! ✓✓✓" -ForegroundColor Green
    Write-Host ""
    Write-Host "Sample response:" -ForegroundColor Cyan
    $response | ConvertTo-Json -Depth 3
    Write-Host ""
    Write-Host "=== Your Token is Ready to Use ===" -ForegroundColor Green
    Write-Host "Authorization: Bearer $token"
    Write-Host ""
    Write-Host "You can now make API requests with this token!" -ForegroundColor Yellow
}
catch {
    $statusCode = $_.Exception.Response.StatusCode.value__
    Write-Host "✗ Request failed with status code: $statusCode" -ForegroundColor Red
    
    if ($statusCode -eq 401) {
        Write-Host ""
        Write-Host "Still getting 401. Possible issues:" -ForegroundColor Yellow
        Write-Host "1. API not fully restarted yet - wait a few seconds and try again"
        Write-Host "2. Token claims are missing required fields"
        Write-Host ""
        Write-Host "Retrying in 3 seconds..." -ForegroundColor Cyan
        Start-Sleep -Seconds 3
        
        try {
            $response = Invoke-RestMethod -Uri "http://localhost:5271/api/Data/Customer?limit=1" -Method Get -Headers $headers -ErrorAction Stop
            Write-Host "✓ SUCCESS on retry!" -ForegroundColor Green
            $response | ConvertTo-Json -Depth 3
        }
        catch {
            Write-Host "✗ Still failing. Error: $($_.Exception.Message)" -ForegroundColor Red
            if ($_.ErrorDetails.Message) {
                $_.ErrorDetails.Message
            }
        }
    }
    else {
        Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
        if ($_.ErrorDetails.Message) {
            Write-Host ""
            Write-Host "Response:" -ForegroundColor Yellow
            $_.ErrorDetails.Message
        }
    }
}
