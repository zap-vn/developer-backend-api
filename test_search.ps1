$body = @{ 
    Skip   = 0
    Limit  = 5
    Filter = @(
        @{ 
            SearchKey = "Search"
            Value     = "test" 
        }
    )
}
$json = $body | ConvertTo-Json
try {
    $response = Invoke-RestMethod -Uri "http://localhost:5271/api/customers" -Method Post -Body $json -ContentType "application/json"
    $response | ConvertTo-Json -Depth 5
}
catch {
    $_.Exception.Message
    if ($_.ErrorDetails) { $_.ErrorDetails.Message }
}
