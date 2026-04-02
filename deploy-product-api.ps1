# Deploy Product API to Google Cloud Run
# Project: pendogo-v1-6317

$projectName = "pendogo-v1-6317"
$region = "asia-southeast1"
$serviceName = "pendogo-product-api"
$repoName = "CRM-repo"
$imageTag = "latest"
$imageName = "asia-southeast1-docker.pkg.dev/$projectName/$repoName/$serviceName:$imageTag"

Write-Host "🚀 Starting Deployment of Product API to Google Cloud Run..." -ForegroundColor Green
Write-Host "Project ID: $projectName" -ForegroundColor Cyan
Write-Host "Service: $serviceName" -ForegroundColor Cyan

# 1. Build and Push image using Cloud Build
Write-Host "Building Docker image $imageName..." -ForegroundColor Yellow
gcloud builds submit --tag $imageName -f Services/Product/CRM.Product.Api/Dockerfile .

# 2. Deploy to Cloud Run
Write-Host "Deploying $serviceName to Cloud Run..." -ForegroundColor Green
gcloud run deploy $serviceName `
    --image $imageName `
    --platform managed `
    --region $region `
    --allow-unauthenticated `
    --port 8080 `
    --set-env-vars "ConnectionStrings__PostgreSql=Host=136.118.121.105;Port=5432;Username=postgres;Password=Pg@Secret2026!;Database=zap_ecosystem,ASPNETCORE_ENVIRONMENT=Production,Jwt:Secret=ThisVerifySecretMustBeLongEnoughForProductApiAndNotSharedInPublicRepo1234567890" `
    --quiet

Write-Host "✅ Deployment Complete!" -ForegroundColor Green
