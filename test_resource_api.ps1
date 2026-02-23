$url = "http://localhost:5000/api/Resources/setup-metadata"

# Create a dummy JWT token with UserGuid claim (since signature validation is disabled)
# Format: { "alg": "none" }.{ "UserGuid": "Customer/1" }.
$header = [Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes('{"alg":"HS256","typ":"JWT"}')).Replace('=', '').Replace('+', '-').Replace('/', '_')
$payload = [Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes('{"UserGuid":"Customer/1","nbf":1614000000,"exp":2147483647}')).Replace('=', '').Replace('+', '-').Replace('/', '_')
$token = "$header.$payload.dummy_signature"

$headers = @{
    "Content-Type"  = "application/json"
    "Authorization" = "Bearer $token"
}

$body = @{
    Data = @(
        @{ _id = "CRMResourceMaps/103" }, # GroupEmployee
        @{ _id = "CRMResourceMaps/261" }  # SystemGender
    )
} | ConvertTo-Json -Depth 5

try {
    Write-Host "Sending request to $url with UserGuid: Customer/1..."
    $response = Invoke-RestMethod -Uri $url -Method Post -Headers $headers -Body $body
    Write-Host "`nResponse Received:"
    $response | ConvertTo-Json -Depth 10
}
catch {
    Write-Error "Error: $_"
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $errorBody = $reader.ReadToEnd()
        Write-Host "Error Body: $errorBody"
    }
}
