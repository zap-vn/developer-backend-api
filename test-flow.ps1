$authPath = "d:\PROJECTS\2026\3_2\src\Services\Authentication\ZAP.Authentication.Api\ZAP.Authentication.Api.csproj"
$custPath = "d:\PROJECTS\2026\3_2\src\Services\Customer\ZAP.Customer.Api\ZAP.Customer.Api.csproj"

$authProcess = Start-Process -FilePath "dotnet" -ArgumentList "run --project $authPath" -PassThru -NoNewWindow
$custProcess = Start-Process -FilePath "dotnet" -ArgumentList "run --project $custPath" -PassThru -NoNewWindow

Write-Host "Waiting 15 seconds for APIs to initialize..."
Start-Sleep -Seconds 15

try {
    $body = @{
        merchantName = "Acme Test Corp"
        email = "test@acmecorp.com"
        username = "acme_admin"
        password = "securePassword123!"
    } | ConvertTo-Json

    Write-Host "Submitting API Request to Auth..."
    $res = Invoke-RestMethod -Method Post -Uri "http://localhost:5001/api/auth/register-merchant" -Body $body -ContentType "application/json"
    
    Write-Host "Response received:"
    $res | ConvertTo-Json -Depth 5

    Write-Host "Checking if Customer was successfully mapped..."
    $customerCheck = Invoke-RestMethod -Method Post -Uri "http://localhost:5003/api/customers/list" -Body "{}" -ContentType "application/json"
    Write-Host "Customers Collection State:"
    $customerCheck | ConvertTo-Json -Depth 5
} catch {
    Write-Host "Error during execution: $_"
}

Write-Host "Stopping API servers..."
Stop-Process -Id $authProcess.Id -Force
Stop-Process -Id $custProcess.Id -Force
