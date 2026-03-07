#!/bin/bash

# GCP Configuration
PROJECT_ID="your-gcp-project-id"  # Thay bằng GCP Project ID của bạn
REGION="asia-southeast1"          # Singapore region (gần VN nhất)
SERVICE_NAME="CRM-identity-api"
IMAGE_NAME="gcr.io/${PROJECT_ID}/${SERVICE_NAME}"

echo "=== Deploying CRM Identity API to GCP Cloud Run ==="
echo "Project: ${PROJECT_ID}"
echo "Region: ${REGION}"
echo "Service: ${SERVICE_NAME}"
echo ""

# Step 1: Build Docker image
echo "Step 1: Building Docker image..."
docker build -t ${IMAGE_NAME}:latest -f services/CRM.Identity.Api/Dockerfile .

if [ $? -ne 0 ]; then
    echo "❌ Docker build failed!"
    exit 1
fi

echo "✅ Docker build successful!"
echo ""

# Step 2: Push to Google Container Registry
echo "Step 2: Pushing image to GCR..."
docker push ${IMAGE_NAME}:latest

if [ $? -ne 0 ]; then
    echo "❌ Docker push failed!"
    exit 1
fi

echo "✅ Image pushed to GCR!"
echo ""

# Step 3: Deploy to Cloud Run
echo "Step 3: Deploying to Cloud Run..."
gcloud run deploy ${SERVICE_NAME} \
    --image ${IMAGE_NAME}:latest \
    --platform managed \
    --region ${REGION} \
    --allow-unauthenticated \
    --port 8080 \
    --memory 512Mi \
    --cpu 1 \
    --min-instances 0 \
    --max-instances 10 \
    --set-env-vars "ASPNETCORE_ENVIRONMENT=Production" \
    --project ${PROJECT_ID}

if [ $? -ne 0 ]; then
    echo "❌ Cloud Run deployment failed!"
    exit 1
fi

echo ""
echo "✅ Deployment successful!"
echo ""
echo "Your API is now running at:"
gcloud run services describe ${SERVICE_NAME} --region ${REGION} --format 'value(status.url)' --project ${PROJECT_ID}
