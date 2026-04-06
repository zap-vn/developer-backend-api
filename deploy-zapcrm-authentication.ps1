# deploy-zapcrm-authentication.ps1
$PROJECT_ID = "zapcrm-492101"
$REGION = "asia-southeast1"
$SERVICE_NAME = "zapcrm-authentication-api"
$IMAGE_PATH = "asia-southeast1-docker.pkg.dev/$PROJECT_ID/crm-repo/$SERVICE_NAME:latest"

Write-Host "--- 1. Building Docker Image for Authentication Service ---" -ForegroundColor Cyan
gcloud builds submit --tag $IMAGE_PATH .

Write-Host "--- 2. Deploying to Cloud Run ---" -ForegroundColor Cyan
gcloud run deploy $SERVICE_NAME `
  --image $IMAGE_PATH `
  --platform managed `
  --region $REGION `
  --project $PROJECT_ID `
  --no-allow-unauthenticated `
  --set-env-vars "ASPNETCORE_ENVIRONMENT=Production"

Write-Host "SUCCESS: Authentication Service Deployed!" -ForegroundColor Green
