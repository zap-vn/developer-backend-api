$regBody = @{ 
    Email        = "test_dev@zap.vn"
    Password     = "Password123!"
    FirstName    = "Dev"
    LastName     = "Test"
    MerchantName = "TestMerchant" 
}
$json = $regBody | ConvertTo-Json
try {
    $response = Invoke-RestMethod -Uri "http://localhost:5271/api/Auth/register" -Method Post -Body $json -ContentType "application/json"
    $response | ConvertTo-Json
}
catch {
    $_.Exception.Message
    $_.ErrorDetails.Message
}
