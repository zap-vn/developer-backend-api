$root = "d:\PROJECTS\4_2026\01042026"
$dotnet = "C:\Program Files\dotnet\dotnet.exe"

$services = @(
    @{ Name = "Authentication"; Port = 5001; Path = "$root\Services\Authentication\CRM.Authentication.Api" },
    @{ Name = "Product";        Port = 56335; Path = "$root\Services\Product\CRM.Product.Api" },
    @{ Name = "Customer";       Port = 5003; Path = "$root\Services\Customer\CRM.Customer.Api" },
    @{ Name = "Sales";          Port = 5004; Path = "$root\Services\Sales\CRM.Sales.Api" },
    @{ Name = "HR";             Port = 5002; Path = "$root\Services\HR\CRM.HR.Api" },
    @{ Name = "Organization";   Port = 56338; Path = "$root\Services\Organization\CRM.Organization.Api" },
    @{ Name = "Order";          Port = 56342; Path = "$root\Services\Order\CRM.Order.Api" },
    @{ Name = "Payment";        Port = 56344; Path = "$root\Services\Payment\CRM.Payment.Api" },
    @{ Name = "Report";         Port = 56339; Path = "$root\Services\Report\CRM.Report.Api" },
    @{ Name = "Promotion";      Port = 56345; Path = "$root\Services\Promotion\CRM.Promotion.Api" }
)

$gateway = @{ Name = "Gateway"; Port = 5000; Path = "$root\Gateway\CRM.Gateway.Api" }

Write-Host "--- CRM New Startup ---"
taskkill /IM dotnet.exe /F 2>$null | Out-Null
Start-Sleep -Seconds 2

Write-Host "Building solution..."
& $dotnet build "$root\CRM.sln"

foreach ($s in $services) {
    Write-Host "Starting $($s.Name)..."
    Start-Process -FilePath $dotnet -ArgumentList "run --project `"$($s.Path)`" --no-build" -WorkingDirectory $s.Path -WindowStyle Hidden -RedirectStandardOutput "$($s.Name)_out.txt" -RedirectStandardError "$($s.Name)_err.txt"
}

Write-Host "Starting Gateway..."
Start-Process -FilePath $dotnet -ArgumentList "run --project `"$($gateway.Path)`" --no-build" -WorkingDirectory $gateway.Path -WindowStyle Hidden -RedirectStandardOutput "gateway_out.txt" -RedirectStandardError "gateway_err.txt"

Write-Host "Waiting..."
Start-Sleep -Seconds 10
Write-Host "Done. Check logs or http://localhost:5000"
