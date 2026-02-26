$PROJECT_ID = "openclaw-zap"
$API_ID = "zap-api"
$GATEWAY_ID = "zap-gateway"
$LOCATION = "us-central1"
$CONFIG_ID = "zap-config-v" + (Get-Date -Format "yyyyMMdd-HHmmss")

Write-Host "🚀 Updating API Gateway Config..." -ForegroundColor Green
Write-Host "Project: $PROJECT_ID"
Write-Host "Config ID: $CONFIG_ID"

# 1. Create new API config
Write-Host "Step 1: Creating new API configuration..."
cmd /c "gcloud api-gateway api-configs create $CONFIG_ID --api=$API_ID --openapi-spec=api-gateway.yaml --project=$PROJECT_ID"

if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to create API configuration."
    exit $LASTEXITCODE
}

# 2. Update Gateway to use new config
Write-Host "Step 2: Updating Gateway with new config..."
cmd /c "gcloud api-gateway gateways update $GATEWAY_ID --api=$API_ID --api-config=$CONFIG_ID --location=$LOCATION --project=$PROJECT_ID"

if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to update Gateway."
    exit $LASTEXITCODE
}

Write-Host "✅ Gateway updated successfully!" -ForegroundColor Green
