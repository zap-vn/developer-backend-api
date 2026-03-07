# Deploy Identity API to Google Cloud Run
# Project: openclaw-CRM

Write-Host "🚀 Starting Deployment to Google Cloud Run..." -ForegroundColor Green
Write-Host "Project ID: openclaw-CRM" -ForegroundColor Cyan

# 1. Login if needed
Write-Host "Checking authentication..."
# gcloud auth login

# 2. Set Project
Write-Host "Setting project to openclaw-CRM..."
# gcloud config set project openclaw-CRM

# 3. Enable Required Services
Write-Host "Enabling Google Cloud Services..."
gcloud services enable run.googleapis.com containerregistry.googleapis.com cloudbuild.googleapis.com

# 4. Deploy using the existing image but inject the correct ConnectionString
Write-Host "Deploying Identity API with correct connection string..."
gcloud run deploy CRM-identity-api `
    --image gcr.io/openclaw-CRM/CRM-identity-api:latest `
    --platform managed `
    --region asia-southeast1 `
    --allow-unauthenticated `
    --port 8080 `
    --set-env-vars "MongoDB__ConnectionString=mongodb+srv://tommy_db_user:Tommy123456@cluster0.dcuwhnu.mongodb.net/?appName=Cluster0&connectTimeoutMS=10000&serverSelectionTimeoutMS=10000&socketTimeoutMS=10000,ASPNETCORE_ENVIRONMENT=Development,Jwt:Secret=ThisVerifySecretMustBeLongEnoughForIdentityApiAndNotSharedInPublicRepo1234567890" `
    --quiet

Write-Host "✅ Deployment Complete!" -ForegroundColor Green
