$env:PATH = "C:\Program Files\dotnet;" + $env:PATH
$dotnetExe = "C:\Program Files\dotnet\dotnet.exe"

$services = @{
    "Gateway" = @{ path = "Gateway\CRM.Gateway.Api\CRM.Gateway.Api.csproj"; port = 5000 }
    "Auth"    = @{ path = "Services\Authentication\CRM.Authentication.Api\CRM.Authentication.Api.csproj"; port = 5001 }
    "Customer" = @{ path = "Services\Customer\CRM.Customer.Api\CRM.Customer.Api.csproj"; port = 5002 }
    "Product"  = @{ path = "Services\Product\CRM.Product.Api\CRM.Product.Api.csproj"; port = 5003 }
    "Order"    = @{ path = "Services\Order\CRM.Order.Api\CRM.Order.Api.csproj"; port = 5004 }
    "Promo"    = @{ path = "Services\Promotion\CRM.Promotion.Api\CRM.Promotion.Api.csproj"; port = 5005 }
    "Payment"  = @{ path = "Services\Payment\CRM.Payment.Api\CRM.Payment.Api.csproj"; port = 5006 }
}

Write-Host "🚀 Cleaning up existing processes..."
Get-Process dotnet -ErrorAction SilentlyContinue | Stop-Process -Force
Get-Process CRM.Customer.Api -ErrorAction SilentlyContinue | Stop-Process -Force

foreach ($name in $services.Keys) {
    $service = $services[$name]
    $fullPath = "d:\PROJECTS\4_2026\01042026\$($service.path)"
    
    # Check for Single-File Exe workaround
    $dirName = [System.IO.Path]::GetDirectoryName($service.path)
    $projectName = [System.IO.Path]::GetFileNameWithoutExtension($service.path)
    $publishExe = "d:\PROJECTS\4_2026\01042026\$($dirName)\publish_single\$($projectName).exe"

    if (Test-Path $publishExe) {
        Write-Host "🚀 Starting $name on port $($service.port) using Single-File EXE..."
        Start-Process -FilePath $publishExe -ArgumentList "--urls http://localhost:$($service.port)" -NoNewWindow -RedirectStandardOutput "$($name.ToLower())_out.txt" -RedirectStandardError "$($name.ToLower())_err.txt"
    } else {
        Write-Host "🚀 Starting $name on port $($service.port) with --no-build..."
        Start-Process -FilePath $dotnetExe -ArgumentList "run --project `"$fullPath`" --urls http://localhost:$($service.port) --no-build" -NoNewWindow -RedirectStandardOutput "$($name.ToLower())_out.txt" -RedirectStandardError "$($name.ToLower())_err.txt"
    }
}

Write-Host "✅ All services starting. Waiting 15s to settle..."
Start-Sleep -Seconds 15

foreach ($name in $services.Keys) {
    $service = $services[$name]
    Write-Host "$name is running on: http://localhost:$($service.port)"
}

