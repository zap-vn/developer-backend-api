# Deploy zapcrm Promotion API to Google Cloud Run
# Project: zapcrm-492101

$projectName = "zapcrm-492101"
$region = "asia-southeast1"
$serviceName = "zapcrm-promotion-api"
$repoName = "crm-repo"
$imageTag = "latest"
$imageName = "asia-southeast1-docker.pkg.dev/$projectName/$repoName/${serviceName}:$imageTag"

Write-Host "🚀 Starting Deployment of zapcrm-promotion-api to Google Cloud Run..." -ForegroundColor Green
Write-Host "Project ID: $projectName" -ForegroundColor Cyan
Write-Host "Service: $serviceName" -ForegroundColor Cyan

# 1. Build and Push image using Cloud Build
Write-Host "Building Docker image using Cloud Build..." -ForegroundColor Yellow
gcloud builds submit --config cloudbuild.zapcrm-promotion.yaml .

# 2. Deploy to Cloud Run
Write-Host "Deploying $serviceName to Cloud Run..." -ForegroundColor Green
gcloud run deploy $serviceName `
    --image $imageName `
    --platform managed `
    --region $region `
    --allow-unauthenticated `
    --port 8080 `
    --set-env-vars "MongoSettings__ConnectionString=mongodb://tommy_db_user:Tommy123456@ac-ewrdepk-shard-00-00.dcuwhnu.mongodb.net:27017,ac-ewrdepk-shard-00-01.dcuwhnu.mongodb.net:27017,ac-ewrdepk-shard-00-02.dcuwhnu.mongodb.net:27017/SinglePoint_en?ssl=true&authSource=admin&retryWrites=true&w=majority,MongoSettings__DatabaseName=SinglePoint_en,ASPNETCORE_ENVIRONMENT=Production,Jwt__Secret=a_very_secret_default_key_at_least_32_chars_long_1234567890" `
    --quiet

Write-Host "✅ Deployment Complete!" -ForegroundColor Green
