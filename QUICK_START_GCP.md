# 🚀 Quick Start: Deploy to GCP Cloud Run

## ⚠️ QUAN TRỌNG: MongoDB Connection

API hiện tại kết nối tới MongoDB tại `172.16.10.153:27017` (IP nội bộ).
Cloud Run sẽ KHÔNG thể kết nối được!

### Giải pháp:
1. **MongoDB Atlas** (Khuyến nghị - Free tier available)
2. **Expose MongoDB ra internet** với IP public
3. **Cloud SQL for MongoDB** trên GCP

---

## 📋 Checklist Deploy

### 1. Cài đặt Google Cloud SDK
- Download: https://cloud.google.com/sdk/docs/install
- Sau khi cài, chạy:
  ```powershell
  gcloud init
  gcloud auth login
  ```

### 2. Tạo GCP Project
- Truy cập: https://console.cloud.google.com/
- Click "New Project"
- Lưu lại **Project ID** (ví dụ: `zap-backend-prod`)

### 3. Enable APIs
```powershell
gcloud services enable run.googleapis.com
gcloud services enable containerregistry.googleapis.com
gcloud services enable cloudbuild.googleapis.com
```

### 4. Cấu hình MongoDB (QUAN TRỌNG!)

#### Option A: MongoDB Atlas (Khuyến nghị)
1. Truy cập: https://www.mongodb.com/cloud/atlas/register
2. Tạo free cluster
3. Whitelist IP: `0.0.0.0/0` (cho phép Cloud Run kết nối)
4. Lấy connection string
5. Update `appsettings.json`:
   ```json
   "ConnectionString": "mongodb+srv://username:password@cluster.mongodb.net/?retryWrites=true&w=majority"
   ```

#### Option B: Expose MongoDB hiện tại (Không khuyến nghị)
- Cần cấu hình firewall để expose port 27017
- Thêm authentication
- Rủi ro bảo mật cao

### 5. Sửa Project ID trong deploy script
Mở file `deploy-gcp.ps1` và sửa:
```powershell
$PROJECT_ID = "zap-backend-prod"  # Thay bằng Project ID thực tế
```

### 6. Deploy!
```powershell
.\deploy-gcp.ps1
```

---

## 🧪 Test sau khi deploy

Sau khi deploy thành công, bạn sẽ nhận được URL:
```
https://zap-identity-api-xxxxxxxxx-as.a.run.app
```

Test API:
```powershell
curl https://zap-identity-api-xxxxxxxxx-as.a.run.app/api/Data/Product `
  -H "Authorization: Bearer YOUR_TOKEN"
```

---

## 💰 Chi phí ước tính

Cloud Run Free Tier:
- 2 million requests/tháng
- 360,000 GiB-seconds memory/tháng
- 180,000 vCPU-seconds/tháng

Ước tính chi phí: **$0-10/tháng** cho traffic nhỏ

---

## 🆘 Troubleshooting

### Lỗi: MongoDB connection timeout
- Kiểm tra MongoDB connection string
- Whitelist IP `0.0.0.0/0` trong MongoDB Atlas
- Kiểm tra network settings

### Lỗi: Permission denied
```powershell
gcloud auth login
gcloud auth configure-docker
```

### Xem logs
```powershell
gcloud run services logs read zap-identity-api --region asia-southeast1
```

---

## 📞 Support

- GCP Docs: https://cloud.google.com/run/docs
- MongoDB Atlas: https://www.mongodb.com/docs/atlas/
