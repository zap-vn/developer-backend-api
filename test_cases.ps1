
# Bypass token
$header = [Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes('{"alg":"HS256","typ":"JWT"}')).Replace('=', '').Replace('+', '-').Replace('/', '_')
$payload = [Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes('{"UserGuid":"Customer/1","nbf":0,"exp":2147483647}')).Replace('=', '').Replace('+', '-').Replace('/', '_')
$token = "$header.$payload.dummy"

Write-Host "=== CASE 1: Accept-Language = 'tiếng hàn' ===" -ForegroundColor Cyan
$headers1 = @{
    "Authorization"   = "Bearer $token"
    "Accept-Language" = "tiếng hàn"
}
$url1 = "http://localhost:5000/api/Data/Product/Product%2F4022"
try {
    $res1 = Invoke-RestMethod -Uri $url1 -Method Get -Headers $headers1
    Write-Host "Name: $($res1.Name)" -ForegroundColor Green
}
catch {
    Write-Host "Error Case 1: $($_.Exception.Message)"
}

Write-Host "`n=== CASE 2: Accept-Language = 'en' (SystemLanguages) ===" -ForegroundColor Cyan
$headers2 = @{
    "Authorization"   = "Bearer $token"
    "Accept-Language" = "en"
}
$url2 = "http://localhost:5000/api/Data/SystemLanguages"
try {
    $res2 = Invoke-RestMethod -Uri $url2 -Method Get -Headers $headers2
    Write-Host "Response (Top 2 items):"
    $res2 | Select-Object -First 2 | ConvertTo-Json -Depth 5
}
catch {
    Write-Host "Error Case 2: $($_.Exception.Message)"
}
