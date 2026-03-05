# Deploy Report API to Google Cloud Run
# Project: openclaw-zap

Write-Host "🚀 Starting Deployment of Report API to Google Cloud Run..." -ForegroundColor Green
Write-Host "Project ID: openclaw-zap" -ForegroundColor Cyan

# 1. Ensure you are logged in to gcloud
# gcloud auth login

# 2. Set the correct project
# gcloud config set project openclaw-zap

# 3. Enable required services (if not already enabled)
Write-Host "Enabling Google Cloud Services..." -ForegroundColor Yellow
gcloud services enable run.googleapis.com containerregistry.googleapis.com cloudbuild.googleapis.com

# 4. Build and Push the Docker image using Google Cloud Build
$imageName = "asia-southeast1-docker.pkg.dev/openclaw-zap/zap-repo/zap-report-api:latest"
Write-Host "Building Docker image $imageName using Cloud Build..." -ForegroundColor Cyan
gcloud builds submit d:\PROJECTS\2026\3_2\src --config=d:\PROJECTS\2026\3_2\src\cloudbuild.report.yaml


# 6. Deploy to Cloud Run with required environment variables
Write-Host "Deploying Report API..." -ForegroundColor Green
gcloud run deploy zap-report-api `
    --image $imageName `
    --platform managed `
    --region asia-southeast1 `
    --allow-unauthenticated `
    --port 8080 `
    --set-env-vars "MongoDB__ConnectionString=mongodb+srv://tommy_db_user:Tommy123456@cluster0.dcuwhnu.mongodb.net/?appName=Cluster0&connectTimeoutMS=10000&serverSelectionTimeoutMS=10000&socketTimeoutMS=10000,ASPNETCORE_ENVIRONMENT=Development,Jwt:Secret=ThisVerifySecretMustBeLongEnoughForReportApiAndNotSharedInPublicRepo1234567890" `
    --quiet

Write-Host "✅ Report API deployment complete!" -ForegroundColor Green
