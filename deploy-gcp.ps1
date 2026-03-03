# Deploy Identity API to Google Cloud Run
# Project: openclaw-zap

Write-Host "🚀 Starting Deployment to Google Cloud Run..." -ForegroundColor Green
Write-Host "Project ID: openclaw-zap" -ForegroundColor Cyan

# 1. Login if needed
Write-Host "Checking authentication..."
# gcloud auth login

# 2. Set Project
Write-Host "Setting project to openclaw-zap..."
# gcloud config set project openclaw-zap

# 3. Enable Required Services
Write-Host "Enabling Google Cloud Services..."
gcloud services enable run.googleapis.com containerregistry.googleapis.com cloudbuild.googleapis.com

# 4. Build Image using Cloud Build (No local Docker needed)
$IMAGE_TAG = "gcr.io/openclaw-zap/zap-identity-api:latest"
Write-Host "Building Container with Cloud Build..."
gcloud builds submit --tag $IMAGE_TAG .

# 5. Deploy to Cloud Run
Write-Host "Deploying Identity API..."
gcloud run deploy zap-identity-api `
    --image $IMAGE_TAG `
    --platform managed `
    --region asia-southeast1 `
    --allow-unauthenticated `
    --port 8080 `
    --set-env-vars "ASPNETCORE_ENVIRONMENT=Development,JwtSettings:Secret=ThisVerifySecretMustBeLongEnoughForIdentityApiAndNotSharedInPublicRepo1234567890" `
    --quiet

Write-Host "✅ Deployment Complete!" -ForegroundColor Green
