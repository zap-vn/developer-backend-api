
# Bypass login for testing
$header = [Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes('{"alg":"HS256","typ":"JWT"}')).Replace('=', '').Replace('+', '-').Replace('/', '_')
$payload = [Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes('{"UserGuid":"Customer/1","nbf":0,"exp":2147483647}')).Replace('=', '').Replace('+', '-').Replace('/', '_')
$token = "$header.$payload.dummy"

$headers = @{
    "Authorization"   = "Bearer $token"
    "Accept-Language" = "ko"
}

$testUrl = "http://localhost:5000/api/Data/Product/Product%2F4022"
Write-Host "Testing Korean localization at: $testUrl"
try {
    $testResponse = Invoke-RestMethod -Uri $testUrl -Method Get -Headers $headers
    Write-Host "=== API RESPONSE (KO) ===" -ForegroundColor Green
    $testResponse | ConvertTo-Json -Depth 5
}
catch {
    Write-Host "Error: $($_.Exception.Message)"
}
