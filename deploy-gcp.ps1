# Deploy Identity API to Google Cloud Run
# Project: openclaw-zap

Write-Host "🚀 Starting Deployment to Google Cloud Run..." -ForegroundColor Green
Write-Host "Project ID: openclaw-zap" -ForegroundColor Cyan

# 1. Login if needed
Write-Host "Checking authentication..."
gcloud auth login

# 2. Set Project
Write-Host "Setting project to openclaw-zap..."
gcloud config set project openclaw-zap

# 3. Enable Required Services
Write-Host "Enabling Google Cloud Services..."
gcloud services enable run.googleapis.com containerregistry.googleapis.com cloudbuild.googleapis.com

# 4. Deploy Identity API
Write-Host "Deploying Identity API..."
gcloud run deploy zap-identity-api `
    --source . `
    --platform managed `
    --region asia-southeast1 `
    --allow-unauthenticated `
    --port 8080 `
    --set-env-vars "ASPNETCORE_ENVIRONMENT=Development"

Write-Host "✅ Deployment Complete!" -ForegroundColor Green
