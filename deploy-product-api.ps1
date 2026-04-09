# Deploy Product API to Google Cloud Run
# Project: pendogo-v1-6317

$projectName = "zapcrm-492101"
$region = "asia-southeast1"
$serviceName = "pendogo-product-api"
$repoName = "CRM-repo"
$imageTag = "latest"
$imageName = "asia-southeast1-docker.pkg.dev/${projectName}/${repoName}/${serviceName}:${imageTag}"

Write-Host "🚀 Starting Deployment of Product API to Google Cloud Run..." -ForegroundColor Green
Write-Host "Project ID: $projectName" -ForegroundColor Cyan
Write-Host "Service: $serviceName" -ForegroundColor Cyan

# 1. Build, Push and Deploy using Cloud Build Config
Write-Host "Triggering Cloud Build for $serviceName..." -ForegroundColor Yellow
gcloud builds submit . `
    --config=cloudbuild_product_deploy.yaml `
    --substitutions=_PROJECT_ID=$projectName,_REGION=$region,_SERVICE_NAME=$serviceName,_IMAGE=$imageName

Write-Host "✅ Deployment Complete!" -ForegroundColor Green
