# GCP Configuration
$PROJECT_ID = "your-gcp-project-id"  # Thay bằng GCP Project ID của bạn
$REGION = "asia-southeast1"          # Singapore region (gần VN nhất)
$SERVICE_NAME = "zap-identity-api"
$IMAGE_NAME = "gcr.io/$PROJECT_ID/$SERVICE_NAME"

Write-Host "=== Deploying Zap Identity API to GCP Cloud Run ===" -ForegroundColor Cyan
Write-Host "Project: $PROJECT_ID" -ForegroundColor Yellow
Write-Host "Region: $REGION" -ForegroundColor Yellow
Write-Host "Service: $SERVICE_NAME" -ForegroundColor Yellow
Write-Host ""

# Step 1: Build Docker image
Write-Host "Step 1: Building Docker image..." -ForegroundColor Cyan
docker build -t "${IMAGE_NAME}:latest" -f services/Zap.Identity.Api/Dockerfile .

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Docker build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Docker build successful!" -ForegroundColor Green
Write-Host ""

# Step 2: Configure Docker to use gcloud as credential helper
Write-Host "Step 2: Configuring Docker authentication..." -ForegroundColor Cyan
gcloud auth configure-docker

# Step 3: Push to Google Container Registry
Write-Host "Step 3: Pushing image to GCR..." -ForegroundColor Cyan
docker push "${IMAGE_NAME}:latest"

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Docker push failed!" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Image pushed to GCR!" -ForegroundColor Green
Write-Host ""

# Step 4: Deploy to Cloud Run
Write-Host "Step 4: Deploying to Cloud Run..." -ForegroundColor Cyan
gcloud run deploy $SERVICE_NAME `
    --image "${IMAGE_NAME}:latest" `
    --platform managed `
    --region $REGION `
    --allow-unauthenticated `
    --port 8080 `
    --memory 512Mi `
    --cpu 1 `
    --min-instances 0 `
    --max-instances 10 `
    --set-env-vars "ASPNETCORE_ENVIRONMENT=Production" `
    --project $PROJECT_ID

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Cloud Run deployment failed!" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "✅ Deployment successful!" -ForegroundColor Green
Write-Host ""
Write-Host "Your API is now running at:" -ForegroundColor Cyan
$url = gcloud run services describe $SERVICE_NAME --region $REGION --format 'value(status.url)' --project $PROJECT_ID
Write-Host $url -ForegroundColor Yellow
