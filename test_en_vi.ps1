
$header = [Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes('{"alg":"HS256","typ":"JWT"}')).Replace('=', '').Replace('+', '-').Replace('/', '_')
$payload = [Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes('{"UserGuid":"Customer/1","nbf":0,"exp":2147483647}')).Replace('=', '').Replace('+', '-').Replace('/', '_')
$token = "$header.$payload.dummy"

Write-Host "=== TESTING EN ===" -ForegroundColor Cyan
$headers = @{
    "Authorization"   = "Bearer $token"
    "Accept-Language" = "en"
}
$url = "http://localhost:5000/api/Data/Product/Product%2F4022"
try {
    $res = Invoke-RestMethod -Uri $url -Method Get -Headers $headers
    $res | ConvertTo-Json -Depth 2
}
catch {
    Write-Host "Error: $($_.Exception.Message)"
}

Write-Host "`n=== TESTING VI ===" -ForegroundColor Cyan
$headers2 = @{
    "Authorization"   = "Bearer $token"
    "Accept-Language" = "vi"
}
try {
    $res2 = Invoke-RestMethod -Uri $url -Method Get -Headers $headers2
    $res2 | ConvertTo-Json -Depth 2
}
catch {
    Write-Host "Error: $($_.Exception.Message)"
}
