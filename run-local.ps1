$env:PATH = "C:\Program Files\dotnet;" + $env:PATH
$gatewayPath = "d:\PROJECTS\4_2026\01042026\Gateway\CRM.Gateway.Api\CRM.Gateway.Api.csproj"
$authPath = "d:\PROJECTS\4_2026\01042026\Services\Authentication\CRM.Authentication.Api\CRM.Authentication.Api.csproj"
$dotnetExe = "C:\Program Files\dotnet\dotnet.exe"

Write-Host "🚀 Starting Gateway..."
$gatewayProcess = Start-Process -FilePath $dotnetExe -ArgumentList "run --project $gatewayPath --urls http://localhost:5000" -PassThru -NoNewWindow -RedirectStandardOutput "gateway_out.txt" -RedirectStandardError "gateway_err.txt"
Write-Host "🚀 Starting Authentication..."
$authProcess = Start-Process -FilePath $dotnetExe -ArgumentList "run --project $authPath --urls http://localhost:5001" -PassThru -NoNewWindow -RedirectStandardOutput "auth_out.txt" -RedirectStandardError "auth_err.txt"

Write-Host "Waiting logs to settle (10s)..."
Start-Sleep -Seconds 10

Write-Host "Processes: Gateway (ID: $($gatewayProcess.Id)), Auth (ID: $($authProcess.Id))"
Write-Host "Gateway: http://localhost:5000"
Write-Host "Auth: http://localhost:5001"
