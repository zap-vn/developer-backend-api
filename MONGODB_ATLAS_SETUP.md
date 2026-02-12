# 🍃 MongoDB Atlas Setup Guide (Free Tier)

## Tại sao dùng MongoDB Atlas?

- ✅ **Free tier**: 512MB storage miễn phí mãi mãi
- ✅ **Managed service**: Không cần quản lý server
- ✅ **High availability**: Auto backup, replica sets
- ✅ **Global**: Có server ở Singapore (gần VN)
- ✅ **Cloud Run compatible**: Public IP, dễ kết nối

---

## 📋 Bước 1: Tạo tài khoản MongoDB Atlas

1. **Truy cập:** https://www.mongodb.com/cloud/atlas/register

2. **Đăng ký** bằng:
   - Email
   - Google account
   - GitHub account

3. **Verify email** (nếu dùng email)

---

## 📋 Bước 2: Tạo Organization & Project

1. Sau khi đăng nhập, click **"Create an Organization"**
   - Organization Name: `Zap Backend`
   - Click **"Next"**

2. **Add members** (optional, có thể skip)
   - Click **"Create Organization"**

3. **Create a Project:**
   - Project Name: `Zap Production`
   - Click **"Next"** → **"Create Project"**

---

## 📋 Bước 3: Tạo Free Cluster

1. Click **"Build a Database"** hoặc **"Create"**

2. **Chọn plan:**
   - Chọn **"M0 FREE"** (512MB)
   - ⚠️ Đừng chọn M10 trở lên (tốn tiền!)

3. **Chọn Cloud Provider & Region:**
   - Provider: **AWS** (khuyến nghị)
   - Region: **Singapore (ap-southeast-1)** (gần VN nhất)
   - Cluster Name: `zap-cluster-free`

4. Click **"Create Cluster"**
   - Đợi 3-5 phút để cluster được tạo

---

## 📋 Bước 4: Tạo Database User

1. Trong tab **"Security"** → **"Database Access"**

2. Click **"Add New Database User"**

3. **Authentication Method:** Password
   - Username: `zap_admin`
   - Password: **Tạo password mạnh** (click "Autogenerate Secure Password")
   - ⚠️ **LƯU LẠI PASSWORD NÀY!** Bạn sẽ cần nó sau

4. **Database User Privileges:**
   - Chọn **"Read and write to any database"**

5. Click **"Add User"**

---

## 📋 Bước 5: Whitelist IP Addresses

1. Trong tab **"Security"** → **"Network Access"**

2. Click **"Add IP Address"**

3. **Cho phép tất cả IP** (để Cloud Run kết nối được):
   - Click **"Allow Access from Anywhere"**
   - IP Address: `0.0.0.0/0`
   - Comment: `Allow Cloud Run and all services`

4. Click **"Confirm"**

⚠️ **Lưu ý bảo mật:** 
- Trong production, nên whitelist specific IP ranges
- Đảm bảo database user có password mạnh

---

## 📋 Bước 6: Lấy Connection String

1. Quay lại **"Database"** tab

2. Click **"Connect"** trên cluster của bạn

3. Chọn **"Connect your application"**

4. **Driver:** C# / .NET
   **Version:** 2.13 or later

5. **Copy Connection String:**
   ```
   mongodb+srv://zap_admin:<password>@zap-cluster-free.xxxxx.mongodb.net/?retryWrites=true&w=majority
   ```

6. **Thay `<password>`** bằng password thực tế:
   ```
   mongodb+srv://zap_admin:YourActualPassword123@zap-cluster-free.xxxxx.mongodb.net/?retryWrites=true&w=majority
   ```

---

## 📋 Bước 7: Import dữ liệu từ MongoDB hiện tại

### Option A: Dùng MongoDB Compass (GUI - Dễ nhất)

1. **Download MongoDB Compass:** https://www.mongodb.com/try/download/compass

2. **Kết nối tới MongoDB cũ:**
   - Connection String: `mongodb://172.16.10.153:27017`

3. **Export collections:**
   - Chọn database → collection
   - Click "Export Collection"
   - Format: JSON
   - Lưu file

4. **Kết nối tới MongoDB Atlas:**
   - Connection String: (string từ bước 6)

5. **Import collections:**
   - Chọn database → collection
   - Click "Add Data" → "Import File"
   - Chọn file JSON đã export

### Option B: Dùng mongodump/mongorestore (CLI)

```powershell
# Export từ MongoDB cũ
mongodump --uri="mongodb://172.16.10.153:27017" --out="./backup"

# Import vào MongoDB Atlas
mongorestore --uri="mongodb+srv://zap_admin:password@cluster.mongodb.net" ./backup
```

---

## 📋 Bước 8: Update Connection String trong API

1. **Mở file:** `services/Zap.Identity.Api/appsettings.json`

2. **Update ConnectionString:**

```json
{
  "DatabaseSettings": {
    "DBProvider": "mongodb",
    "ConnectionString": "mongodb+srv://zap_admin:YourPassword@zap-cluster-free.xxxxx.mongodb.net/?retryWrites=true&w=majority",
    "DatabaseName": "SinglePoint_en",
    "Databases": {
      "Identity": "SinglePoint_en",
      "System": "SinglePoint_System",
      "Pos": "SinglePoint_Pos",
      "Orders": "SinglePoint_Orders_vi",
      "Warehouse": "SinglePoint_Warehouse",
      "Hr": "SinglePoint_Hr",
      "Payment": "SinglePoint_Payment"
    }
  }
}
```

3. **⚠️ Bảo mật:** Không commit password vào Git!

### Dùng User Secrets (Khuyến nghị):

```powershell
cd services/Zap.Identity.Api

# Set connection string as secret
dotnet user-secrets set "DatabaseSettings:ConnectionString" "mongodb+srv://zap_admin:password@cluster.mongodb.net/?retryWrites=true&w=majority"
```

---

## 📋 Bước 9: Test kết nối

1. **Chạy API local:**
   ```powershell
   dotnet run --project services/Zap.Identity.Api/Zap.Identity.Api.csproj
   ```

2. **Test endpoint:**
   ```powershell
   curl http://localhost:5271/api/Data/Product -H "Authorization: Bearer YOUR_TOKEN"
   ```

3. **Kiểm tra logs** để đảm bảo kết nối thành công

---

## 📋 Bước 10: Deploy lên GCP với MongoDB Atlas

1. **Update appsettings.Production.json:**

```json
{
  "DatabaseSettings": {
    "ConnectionString": "mongodb+srv://zap_admin:password@cluster.mongodb.net/?retryWrites=true&w=majority"
  }
}
```

2. **Hoặc dùng Environment Variables trong Cloud Run:**

```powershell
gcloud run services update zap-identity-api \
  --region asia-southeast1 \
  --set-env-vars="DatabaseSettings__ConnectionString=mongodb+srv://..."
```

3. **Hoặc dùng Secret Manager (Khuyến nghị nhất):**

```powershell
# Tạo secret
echo -n "mongodb+srv://zap_admin:password@cluster.mongodb.net/..." | gcloud secrets create mongodb-connection-string --data-file=-

# Deploy với secret
gcloud run deploy zap-identity-api \
  --set-secrets="DatabaseSettings__ConnectionString=mongodb-connection-string:latest"
```

---

## 🎯 Checklist hoàn thành

- [ ] Tạo MongoDB Atlas account
- [ ] Tạo free cluster (M0) ở Singapore
- [ ] Tạo database user với password mạnh
- [ ] Whitelist IP `0.0.0.0/0`
- [ ] Lấy connection string
- [ ] Import dữ liệu từ MongoDB cũ
- [ ] Update connection string trong API
- [ ] Test kết nối local
- [ ] Deploy lên GCP với connection string mới

---

## 💡 Tips

### 1. Monitor Usage
- Truy cập Atlas Dashboard
- Xem **"Metrics"** tab để theo dõi:
  - Storage usage (max 512MB free)
  - Connection count
  - Operations per second

### 2. Backup
- Free tier có **automatic daily backups**
- Giữ trong 2 ngày
- Có thể restore bất cứ lúc nào

### 3. Performance
- Free tier có giới hạn:
  - Shared CPU
  - 512MB RAM
  - 512MB storage
- Nếu cần nhiều hơn, upgrade lên M10 ($0.08/hour)

### 4. Security Best Practices
- ✅ Dùng strong password
- ✅ Enable 2FA cho Atlas account
- ✅ Rotate passwords định kỳ
- ✅ Dùng Secret Manager cho production
- ❌ Không commit connection string vào Git

---

## 🆘 Troubleshooting

### Lỗi: "Authentication failed"
- Kiểm tra username/password
- Đảm bảo user có quyền "Read and write to any database"

### Lỗi: "Connection timeout"
- Kiểm tra IP whitelist (phải có `0.0.0.0/0`)
- Kiểm tra firewall/network

### Lỗi: "Database not found"
- Database sẽ tự động tạo khi insert data lần đầu
- Hoặc tạo thủ công trong Atlas UI

### Cluster bị pause
- Free tier clusters tự động pause sau 60 ngày không hoạt động
- Click "Resume" để kích hoạt lại

---

## 📞 Support

- **MongoDB Atlas Docs:** https://www.mongodb.com/docs/atlas/
- **Community Forum:** https://www.mongodb.com/community/forums/
- **Support:** https://support.mongodb.com/ (paid plans only)

---

## 💰 Pricing

**Free Tier (M0):**
- Storage: 512MB
- RAM: Shared
- vCPU: Shared
- Cost: **$0/month** ✅

**Paid Tiers:**
- M10: $0.08/hour (~$57/month)
- M20: $0.20/hour (~$146/month)
- M30: $0.54/hour (~$394/month)

Free tier đủ cho:
- Development
- Testing
- Small production apps (<10k users)
