# Script cài đặt Google Cloud SDK cho Windows

Write-Host "=== Installing Google Cloud SDK ===" -ForegroundColor Cyan

# Download installer
$installerUrl = "https://dl.google.com/dl/cloudsdk/channels/rapid/GoogleCloudSDKInstaller.exe"
$installerPath = "$env:TEMP\GoogleCloudSDKInstaller.exe"

Write-Host "Downloading installer..." -ForegroundColor Yellow
Invoke-WebRequest -Uri $installerUrl -OutFile $installerPath

Write-Host "Running installer..." -ForegroundColor Yellow
Write-Host "Please follow the installation wizard." -ForegroundColor Green
Start-Process -FilePath $installerPath -Wait

Write-Host ""
Write-Host "✅ Installation complete!" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "1. Close and reopen PowerShell" -ForegroundColor Yellow
Write-Host "2. Run: gcloud init" -ForegroundColor Yellow
Write-Host "3. Run: gcloud auth login" -ForegroundColor Yellow
