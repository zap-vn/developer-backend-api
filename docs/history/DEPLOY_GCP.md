# Deploy Zap Identity API lên Google Cloud Platform (GCP)

## Yêu cầu trước khi deploy

### 1. Cài đặt công cụ cần thiết:

- **Docker Desktop**: https://www.docker.com/products/docker-desktop/
- **Google Cloud SDK (gcloud CLI)**: https://cloud.google.com/sdk/docs/install

### 2. Tạo GCP Project:

1. Truy cập: https://console.cloud.google.com/
2. Tạo project mới hoặc chọn project có sẵn
3. Lưu lại **Project ID** (ví dụ: `zap-backend-123456`)

### 3. Enable APIs cần thiết:

```bash
gcloud services enable run.googleapis.com
gcloud services enable containerregistry.googleapis.com
gcloud services enable cloudbuild.googleapis.com
```

### 4. Đăng nhập GCP:

```bash
gcloud auth login
gcloud config set project YOUR_PROJECT_ID
```

## Các bước deploy

### Bước 1: Cấu hình Project ID

Mở file `deploy-gcp.ps1` và sửa dòng:

```powershell
$PROJECT_ID = "your-gcp-project-id"  # Thay bằng GCP Project ID của bạn
```

Thành:

```powershell
$PROJECT_ID = "zap-backend-123456"  # Project ID thực tế của bạn
```

### Bước 2: Build và test Docker image locally (Optional)

```bash
# Build image
docker build -t zap-identity-api:test -f services/Zap.Identity.Api/Dockerfile .

# Test chạy container
docker run -p 8080:8080 zap-identity-api:test

# Test API
curl http://localhost:8080/api/health
```

### Bước 3: Deploy lên Cloud Run

**Trên Windows (PowerShell):**

```powershell
.\deploy-gcp.ps1
```

**Trên Linux/Mac:**

```bash
chmod +x deploy-gcp.sh
./deploy-gcp.sh
```

### Bước 4: Kiểm tra deployment

Sau khi deploy thành công, bạn sẽ nhận được URL dạng:

```
https://zap-identity-api-xxxxxxxxx-as.a.run.app
```

Test API:

```bash
curl https://zap-identity-api-xxxxxxxxx-as.a.run.app/api/health
```

## Cấu hình MongoDB Connection String

### Option 1: Sử dụng Secret Manager (Khuyến nghị)

```bash
# Tạo secret
echo -n "mongodb://172.16.10.153:27017/" | gcloud secrets create mongodb-connection-string --data-file=-

# Deploy với secret
gcloud run deploy zap-identity-api \
    --set-secrets="DatabaseSettings__ConnectionString=mongodb-connection-string:latest"
```

### Option 2: Sử dụng Environment Variables

```bash
gcloud run services update zap-identity-api \
    --region asia-southeast1 \
    --set-env-vars="DatabaseSettings__ConnectionString=mongodb://YOUR_MONGO_HOST:27017/"
```

⚠️ **Lưu ý**: Nếu MongoDB đang chạy trên `172.16.10.153` (IP nội bộ), Cloud Run sẽ không thể kết nối được. Bạn cần:

1. **Deploy MongoDB lên GCP** (Cloud SQL, MongoDB Atlas, hoặc Compute Engine)
2. **Hoặc expose MongoDB ra internet** với IP public (không khuyến nghị vì bảo mật)
3. **Hoặc dùng VPC Connector** để kết nối Cloud Run với VPC nội bộ

## Cấu hình CORS cho production

Sau khi deploy, cập nhật CORS trong `Program.cs` để chỉ cho phép domain cụ thể:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("Production",
        builder => builder.WithOrigins("https://zap-vn.github.io")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials());
});
```

## Monitoring và Logs

### Xem logs:

```bash
gcloud run services logs read zap-identity-api --region asia-southeast1
```

### Xem metrics:

Truy cập: https://console.cloud.google.com/run

## Chi phí ước tính

Cloud Run tính phí theo:
- **CPU time**: ~$0.00002400/vCPU-second
- **Memory**: ~$0.00000250/GiB-second
- **Requests**: $0.40/million requests
- **Free tier**: 2 million requests/tháng

Ước tính: ~$5-20/tháng cho traffic vừa phải

## Troubleshooting

### Lỗi: "permission denied"

```bash
gcloud auth login
gcloud auth configure-docker
```

### Lỗi: "service account does not have permission"

```bash
gcloud projects add-iam-policy-binding YOUR_PROJECT_ID \
    --member="serviceAccount:YOUR_SERVICE_ACCOUNT" \
    --role="roles/run.admin"
```

### Lỗi: Container không start

Xem logs:

```bash
gcloud run services logs read zap-identity-api --region asia-southeast1 --limit 50
```

## Rollback nếu có lỗi

```bash
# Xem các revision
gcloud run revisions list --service zap-identity-api --region asia-southeast1

# Rollback về revision trước
gcloud run services update-traffic zap-identity-api \
    --region asia-southeast1 \
    --to-revisions REVISION_NAME=100
```

## Custom Domain (Optional)

Nếu muốn dùng domain riêng (ví dụ: `api.zap.vn`):

```bash
gcloud run domain-mappings create \
    --service zap-identity-api \
    --domain api.zap.vn \
    --region asia-southeast1
```

Sau đó cấu hình DNS theo hướng dẫn.

---

## Liên hệ

Nếu gặp vấn đề, liên hệ team DevOps hoặc xem docs:
- https://cloud.google.com/run/docs
- https://cloud.google.com/run/docs/quickstarts/build-and-deploy/deploy-dotnet-service
