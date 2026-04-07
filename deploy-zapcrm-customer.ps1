# Deploy zapcrm Customer API to Google Cloud Run
# Project: zapcrm-492101

$projectName = "zapcrm-492101"
$region = "asia-southeast1"
$serviceName = "zapcrm-customer-api"
$repoName = "crm-repo"
$imageTag = "latest"
$imageName = "asia-southeast1-docker.pkg.dev/$projectName/$repoName/${serviceName}:$imageTag"

Write-Host "🚀 Starting Deployment of zapcrm-customer-api to Google Cloud Run..." -ForegroundColor Green
Write-Host "Project ID: $projectName" -ForegroundColor Cyan
Write-Host "Service: $serviceName" -ForegroundColor Cyan

# 1. Build and Push image using Cloud Build
Write-Host "Building Docker image using Cloud Build..." -ForegroundColor Yellow
gcloud builds submit --config cloudbuild.zapcrm-customer.yaml .

# 2. Deploy to Cloud Run
Write-Host "Deploying $serviceName to Cloud Run..." -ForegroundColor Green
gcloud run deploy $serviceName `
    --image $imageName `
    --platform managed `
    --region $region `
    --allow-unauthenticated `
    --port 8080 `
    --set-env-vars "ConnectionStrings__PostgreSql=Host=136.118.121.105;Port=5432;Username=postgres;Password=Pg@Secret2026!;Database=zap_ecosystem_v303,ASPNETCORE_ENVIRONMENT=Production,Jwt:Secret=ThisVerifySecretMustBeLongEnoughForZapCrmProductApiAndNotSharedInPublicRepo1234567890" `
    --quiet

Write-Host "✅ Deployment Complete!" -ForegroundColor Green
