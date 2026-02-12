# 🚀 GCP Cloud Run Deployment - Chi tiết từng bước

## Tổng quan

Cloud Run là serverless container platform của Google Cloud:
- ✅ Auto-scaling (0 → ∞ instances)
- ✅ Pay per use (chỉ trả tiền khi có request)
- ✅ HTTPS tự động
- ✅ Global CDN
- ✅ Zero server management

---

## 🎯 Kiến trúc Deployment

```
Local Code
    ↓
Docker Build (tạo container image)
    ↓
Google Container Registry (lưu trữ image)
    ↓
Cloud Run (chạy container)
    ↓
HTTPS URL (public endpoint)
```

---

## 📋 PHẦN 1: Chuẩn bị môi trường

### Bước 1.1: Cài đặt Google Cloud SDK

**Windows:**
1. Download: https://dl.google.com/dl/cloudsdk/channels/rapid/GoogleCloudSDKInstaller.exe
2. Chạy installer
3. Chọn:
   - ✅ Install bundled Python
   - ✅ Add gcloud to PATH
4. Finish → Mở PowerShell mới

**Verify installation:**
```powershell
gcloud --version
```

Kết quả mong đợi:
```
Google Cloud SDK 460.0.0
bq 2.0.101
core 2024.01.19
gcloud-crc32c 1.0.0
gsutil 5.27
```

### Bước 1.2: Khởi tạo gcloud

```powershell
gcloud init
```

Làm theo hướng dẫn:
1. **Login:** Chọn account Google của bạn
2. **Pick project:** Chọn project hoặc tạo mới
3. **Default region:** Chọn `asia-southeast1` (Singapore)

### Bước 1.3: Authenticate

```powershell
# Login vào Google account
gcloud auth login

# Configure Docker để dùng gcloud credentials
gcloud auth configure-docker
```

### Bước 1.4: Set default project

```powershell
# Xem danh sách projects
gcloud projects list

# Set project mặc định
gcloud config set project YOUR_PROJECT_ID
```

---

## 📋 PHẦN 2: Tạo và cấu hình GCP Project

### Bước 2.1: Tạo project mới (nếu chưa có)

**Qua Web Console:**
1. Truy cập: https://console.cloud.google.com/
2. Click dropdown project → "New Project"
3. Project name: `Zap Backend Production`
4. Project ID: `zap-backend-prod` (phải unique globally)
5. Click "Create"

**Qua CLI:**
```powershell
gcloud projects create zap-backend-prod --name="Zap Backend Production"
gcloud config set project zap-backend-prod
```

### Bước 2.2: Enable Billing

⚠️ **Quan trọng:** Cloud Run cần billing account (nhưng có free tier)

1. Truy cập: https://console.cloud.google.com/billing
2. Link billing account với project
3. Có thể dùng free trial $300 credit

### Bước 2.3: Enable APIs

```powershell
# Enable Cloud Run API
gcloud services enable run.googleapis.com

# Enable Container Registry API
gcloud services enable containerregistry.googleapis.com

# Enable Cloud Build API (optional, for auto-build)
gcloud services enable cloudbuild.googleapis.com

# Enable Secret Manager (for storing connection strings)
gcloud services enable secretmanager.googleapis.com
```

Verify:
```powershell
gcloud services list --enabled
```

---

## 📋 PHẦN 3: Chuẩn bị MongoDB Connection

### Option A: MongoDB Atlas (Khuyến nghị)

Làm theo hướng dẫn trong `MONGODB_ATLAS_SETUP.md`

Connection string mẫu:
```
mongodb+srv://username:password@cluster.mongodb.net/?retryWrites=true&w=majority
```

### Option B: Cloud SQL for MongoDB

```powershell
# Tạo Cloud SQL instance
gcloud sql instances create zap-mongodb \
    --database-version=MONGODB_4_0 \
    --tier=db-n1-standard-1 \
    --region=asia-southeast1
```

### Bước 3.1: Lưu connection string vào Secret Manager

```powershell
# Tạo secret
echo -n "mongodb+srv://user:pass@cluster.mongodb.net/..." | `
  gcloud secrets create mongodb-connection-string --data-file=-

# Verify
gcloud secrets versions access latest --secret="mongodb-connection-string"
```

---

## 📋 PHẦN 4: Build Docker Image

### Bước 4.1: Kiểm tra Dockerfile

File `services/Zap.Identity.Api/Dockerfile` đã được tạo sẵn.

**Giải thích Dockerfile:**

```dockerfile
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# Sử dụng .NET 8 SDK để build

WORKDIR /src
# Set working directory

COPY ["services/Zap.Identity.Api/...csproj", ...]
# Copy project files

RUN dotnet restore
# Restore NuGet packages

COPY . .
# Copy source code

RUN dotnet build -c Release
# Build project

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish
# Publish compiled files

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
# Sử dụng runtime image (nhỏ hơn SDK)

WORKDIR /app
EXPOSE 8080
# Cloud Run yêu cầu port 8080

ENV ASPNETCORE_URLS=http://+:8080
# Set URL binding

COPY --from=publish /app/publish .
# Copy published files từ stage 2

ENTRYPOINT ["dotnet", "Zap.Identity.Api.dll"]
# Run application
```

### Bước 4.2: Test build locally

```powershell
cd D:\PROJECTS\2026\AllCRMAll\Zap.Backend

# Build image
docker build -t zap-identity-api:test -f services/Zap.Identity.Api/Dockerfile .

# Check image
docker images | Select-String "zap-identity-api"
```

### Bước 4.3: Test run locally

```powershell
# Run container
docker run -p 8080:8080 `
  -e "DatabaseSettings__ConnectionString=mongodb://..." `
  zap-identity-api:test

# Test API (trong terminal khác)
curl http://localhost:8080/api/Data/Product
```

Nếu chạy OK → Ready to deploy!

---

## 📋 PHẦN 5: Deploy lên Cloud Run

### Bước 5.1: Sửa deploy script

Mở `deploy-gcp.ps1` và update:

```powershell
$PROJECT_ID = "zap-backend-prod"  # Project ID thực tế của bạn
$REGION = "asia-southeast1"       # Singapore
$SERVICE_NAME = "zap-identity-api"
```

### Bước 5.2: Chạy deploy script

```powershell
.\deploy-gcp.ps1
```

**Script sẽ làm:**
1. Build Docker image
2. Tag image với GCR path
3. Push image lên Google Container Registry
4. Deploy lên Cloud Run
5. Configure service settings

### Bước 5.3: Deploy thủ công (nếu script lỗi)

```powershell
# 1. Build image
docker build -t gcr.io/zap-backend-prod/zap-identity-api:latest `
  -f services/Zap.Identity.Api/Dockerfile .

# 2. Push to GCR
docker push gcr.io/zap-backend-prod/zap-identity-api:latest

# 3. Deploy to Cloud Run
gcloud run deploy zap-identity-api `
  --image gcr.io/zap-backend-prod/zap-identity-api:latest `
  --platform managed `
  --region asia-southeast1 `
  --allow-unauthenticated `
  --port 8080 `
  --memory 512Mi `
  --cpu 1 `
  --min-instances 0 `
  --max-instances 10 `
  --set-secrets="DatabaseSettings__ConnectionString=mongodb-connection-string:latest" `
  --set-env-vars="ASPNETCORE_ENVIRONMENT=Production"
```

### Bước 5.4: Verify deployment

```powershell
# Lấy URL của service
gcloud run services describe zap-identity-api `
  --region asia-southeast1 `
  --format 'value(status.url)'
```

Kết quả:
```
https://zap-identity-api-xxxxxxxxx-as.a.run.app
```

---

## 📋 PHẦN 6: Test Production API

### Bước 6.1: Test health endpoint

```powershell
$API_URL = "https://zap-identity-api-xxxxxxxxx-as.a.run.app"

curl "$API_URL/api/Data/Product" `
  -H "Authorization: Bearer YOUR_TOKEN"
```

### Bước 6.2: Test từ frontend

Update frontend để point tới URL mới:

```javascript
const API_BASE_URL = 'https://zap-identity-api-xxxxxxxxx-as.a.run.app';

fetch(`${API_BASE_URL}/api/Data/Product`, {
  headers: {
    'Authorization': `Bearer ${token}`
  }
})
```

---

## 📋 PHẦN 7: Monitoring & Logs

### Bước 7.1: Xem logs

```powershell
# Real-time logs
gcloud run services logs tail zap-identity-api --region asia-southeast1

# Recent logs
gcloud run services logs read zap-identity-api `
  --region asia-southeast1 `
  --limit 100
```

### Bước 7.2: Xem metrics

**Qua Web Console:**
1. Truy cập: https://console.cloud.google.com/run
2. Click vào service `zap-identity-api`
3. Tab "Metrics":
   - Request count
   - Request latency
   - Container instances
   - Memory usage
   - CPU usage

### Bước 7.3: Set up alerts

```powershell
# Tạo alert khi error rate > 5%
gcloud alpha monitoring policies create `
  --notification-channels=CHANNEL_ID `
  --display-name="High Error Rate" `
  --condition-display-name="Error rate > 5%" `
  --condition-threshold-value=0.05
```

---

## 📋 PHẦN 8: Configuration & Secrets

### Bước 8.1: Update environment variables

```powershell
gcloud run services update zap-identity-api `
  --region asia-southeast1 `
  --set-env-vars="ASPNETCORE_ENVIRONMENT=Production,LOG_LEVEL=Information"
```

### Bước 8.2: Update secrets

```powershell
# Update MongoDB connection string
echo -n "new-connection-string" | `
  gcloud secrets versions add mongodb-connection-string --data-file=-

# Cloud Run sẽ tự động dùng version mới nhất
```

### Bước 8.3: Add JWT secret

```powershell
# Tạo JWT secret
echo -n "your-super-secret-jwt-key-min-32-chars" | `
  gcloud secrets create jwt-secret --data-file=-

# Update service để dùng secret
gcloud run services update zap-identity-api `
  --region asia-southeast1 `
  --set-secrets="JwtSettings__Secret=jwt-secret:latest"
```

---

## 📋 PHẦN 9: Custom Domain (Optional)

### Bước 9.1: Map custom domain

```powershell
gcloud run domain-mappings create `
  --service zap-identity-api `
  --domain api.zap.vn `
  --region asia-southeast1
```

### Bước 9.2: Update DNS

Thêm DNS records theo hướng dẫn:
```
Type: A
Name: api
Value: 216.239.32.21 (hoặc theo hướng dẫn)

Type: AAAA
Name: api
Value: 2001:4860:4802:32::15
```

### Bước 9.3: Verify

```powershell
curl https://api.zap.vn/api/Data/Product
```

---

## 📋 PHẦN 10: CI/CD với GitHub Actions (Bonus)

Tạo `.github/workflows/deploy-gcp.yml`:

```yaml
name: Deploy to Cloud Run

on:
  push:
    branches: [ main ]

jobs:
  deploy:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup Cloud SDK
      uses: google-github-actions/setup-gcloud@v1
      with:
        service_account_key: ${{ secrets.GCP_SA_KEY }}
        project_id: zap-backend-prod
    
    - name: Configure Docker
      run: gcloud auth configure-docker
    
    - name: Build image
      run: |
        docker build -t gcr.io/zap-backend-prod/zap-identity-api:${{ github.sha }} \
          -f services/Zap.Identity.Api/Dockerfile .
    
    - name: Push image
      run: docker push gcr.io/zap-backend-prod/zap-identity-api:${{ github.sha }}
    
    - name: Deploy to Cloud Run
      run: |
        gcloud run deploy zap-identity-api \
          --image gcr.io/zap-backend-prod/zap-identity-api:${{ github.sha }} \
          --region asia-southeast1 \
          --platform managed
```

---

## 💰 Chi phí ước tính

### Free Tier (mỗi tháng):
- 2 million requests
- 360,000 GiB-seconds memory
- 180,000 vCPU-seconds
- 1 GB network egress

### Pricing sau free tier:
- **CPU**: $0.00002400/vCPU-second
- **Memory**: $0.00000250/GiB-second
- **Requests**: $0.40/million requests
- **Network egress**: $0.12/GB

### Ví dụ tính toán:
- 100,000 requests/tháng
- 500ms average response time
- 512MB memory
- = **~$2-5/tháng**

---

## 🆘 Troubleshooting

### Lỗi: "Permission denied"
```powershell
gcloud auth login
gcloud auth application-default login
```

### Lỗi: "Service account does not have permission"
```powershell
gcloud projects add-iam-policy-binding zap-backend-prod \
  --member="serviceAccount:SERVICE_ACCOUNT" \
  --role="roles/run.admin"
```

### Lỗi: "Container failed to start"
- Xem logs: `gcloud run services logs read ...`
- Kiểm tra port 8080
- Kiểm tra environment variables

### Lỗi: "MongoDB connection timeout"
- Kiểm tra MongoDB Atlas IP whitelist
- Verify connection string
- Check secrets configuration

---

## ✅ Checklist Deploy hoàn chỉnh

- [ ] Cài Google Cloud SDK
- [ ] Tạo GCP project
- [ ] Enable APIs (Run, Container Registry, Secret Manager)
- [ ] Setup MongoDB Atlas
- [ ] Lưu connection string vào Secret Manager
- [ ] Test Docker build locally
- [ ] Deploy lên Cloud Run
- [ ] Verify deployment
- [ ] Test production API
- [ ] Setup monitoring & alerts
- [ ] (Optional) Configure custom domain
- [ ] (Optional) Setup CI/CD

---

## 📞 Resources

- **Cloud Run Docs:** https://cloud.google.com/run/docs
- **Pricing Calculator:** https://cloud.google.com/products/calculator
- **Quickstart:** https://cloud.google.com/run/docs/quickstarts/build-and-deploy/deploy-dotnet-service
- **Best Practices:** https://cloud.google.com/run/docs/tips
