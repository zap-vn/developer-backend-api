# 📚 Zap Backend API - Complete Documentation Index

## 🎯 Mục đích

Repository này chứa backend API cho Zap Platform, được xây dựng bằng .NET 8 và MongoDB.

---

## 📖 Documentation Structure

### 🚀 Getting Started
1. **[README.md](README.md)** - Tổng quan project
2. **[QUICK_START_GCP.md](QUICK_START_GCP.md)** - Hướng dẫn nhanh deploy GCP

### 🔧 Setup Guides
3. **[MONGODB_ATLAS_SETUP.md](MONGODB_ATLAS_SETUP.md)** - Setup MongoDB Atlas (Free Tier)
4. **[GCP_DEPLOYMENT_DETAILED.md](GCP_DEPLOYMENT_DETAILED.md)** - Deploy lên GCP Cloud Run chi tiết

### 🐛 Bug Fixes & Issues
5. **[401_FIX_COMPLETE.md](401_FIX_COMPLETE.md)** - Fix 401 Unauthorized errors
6. **[BUG_401_FIXED_VI.md](BUG_401_FIXED_VI.md)** - Giải thích bug 401 (Tiếng Việt)
7. **[TOKEN_ISSUE_EXPLAINED.md](TOKEN_ISSUE_EXPLAINED.md)** - JWT token issues explained

### 🔐 Authentication
8. **[AUTHENTICATION_GUIDE.md](AUTHENTICATION_GUIDE.md)** - Hướng dẫn authentication

### 📝 Pull Requests
9. **[PR_DESCRIPTION.md](PR_DESCRIPTION.md)** - Template cho Pull Request hiện tại

---

## 🏗️ Architecture

```
Zap.Backend/
├── services/
│   ├── Zap.Identity.Api/          # API Layer (Controllers, Program.cs)
│   ├── Zap.Identity.Application/  # Application Layer (DTOs, Interfaces)
│   ├── Zap.Identity.Domain/       # Domain Layer (Entities)
│   └── Zap.Identity.Infrastructure/ # Infrastructure (Repositories, Services)
│
├── deploy-gcp.ps1                 # GCP deployment script (Windows)
├── deploy-gcp.sh                  # GCP deployment script (Linux/Mac)
├── Dockerfile                     # Docker configuration
└── .dockerignore                  # Docker ignore rules
```

---

## 🚀 Quick Start

### 1. Clone repository
```bash
git clone https://github.com/ZAP-vn/Developer-Backend-API.git
cd Zap.Backend
```

### 2. Setup MongoDB
Chọn một trong hai:
- **Local:** MongoDB tại `mongodb://172.16.10.153:27017`
- **Cloud:** Làm theo [MONGODB_ATLAS_SETUP.md](MONGODB_ATLAS_SETUP.md)

### 3. Update connection string
```json
// appsettings.json
{
  "DatabaseSettings": {
    "ConnectionString": "mongodb://your-connection-string"
  }
}
```

### 4. Run API
```bash
dotnet run --project services/Zap.Identity.Api/Zap.Identity.Api.csproj
```

### 5. Test API
```bash
curl http://localhost:5271/api/Data/Product \
  -H "Authorization: Bearer YOUR_TOKEN"
```

---

## 📋 Available Endpoints

### Authentication
- `POST /api/Auth/login` - User login
- `POST /api/Auth/refresh` - Refresh token

### Data Access
- `GET /api/Data/{collectionName}` - Get all documents from collection
- `GET /api/Data/{collectionName}/{id}` - Get document by ID
- `POST /api/Data/{collectionName}` - Create new document
- `PUT /api/Data/{collectionName}/{id}` - Update document
- `DELETE /api/Data/{collectionName}/{id}` - Delete document

### Resources
- `GET /api/Resources/setup-metadata` - Get setup metadata

### Customers
- `GET /api/Customers` - Get all customers
- `GET /api/Customers/{id}` - Get customer by ID
- `PUT /api/Customers/{id}` - Update customer
- `DELETE /api/Customers/{id}` - Delete customer

---

## 🔐 Authentication

API sử dụng JWT Bearer token authentication.

### Get Token
```bash
POST /api/Auth/login
Content-Type: application/json

{
  "UserName": "admin@pho24.vn",
  "Password": "password123",
  "AcceptName": "pho24",
  "IsRemember": true
}
```

### Use Token
```bash
GET /api/Data/Product
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

Chi tiết: [AUTHENTICATION_GUIDE.md](AUTHENTICATION_GUIDE.md)

---

## 🐛 Common Issues & Solutions

### Issue 1: MongoDB Aggregation Error
**Error:** `Expression $ifNull takes exactly 2 arguments. 4 were passed in`

**Solution:** Đã được fix trong commit `c8af760`. Update code và rebuild.

### Issue 2: 401 Unauthorized
**Error:** `401 Unauthorized` khi gọi API

**Solutions:**
1. Kiểm tra JWT token có hợp lệ
2. Kiểm tra token chưa expired
3. Đảm bảo header `Authorization: Bearer TOKEN`

Chi tiết: [401_FIX_COMPLETE.md](401_FIX_COMPLETE.md)

### Issue 3: MongoDB Connection Timeout
**Error:** `MongoConnectionException: Connection timeout`

**Solutions:**
1. Kiểm tra MongoDB đang chạy
2. Verify connection string
3. Check firewall/network settings
4. Nếu dùng Atlas: Whitelist IP `0.0.0.0/0`

---

## 🚀 Deployment

### Local Development
```bash
dotnet run --project services/Zap.Identity.Api/Zap.Identity.Api.csproj
```

### Docker
```bash
docker build -t zap-identity-api -f services/Zap.Identity.Api/Dockerfile .
docker run -p 8080:8080 zap-identity-api
```

### GCP Cloud Run
```bash
# Quick deploy
.\deploy-gcp.ps1

# Or follow detailed guide
```

Chi tiết: [GCP_DEPLOYMENT_DETAILED.md](GCP_DEPLOYMENT_DETAILED.md)

---

## 🧪 Testing

### Unit Tests
```bash
dotnet test
```

### Integration Tests
```bash
# Start API
dotnet run --project services/Zap.Identity.Api/Zap.Identity.Api.csproj

# Run tests
.\test_login.ps1
```

### Manual Testing
- **Postman Collection:** Import từ `postman_collection.json`
- **Swagger UI:** http://localhost:5271/swagger

---

## 📊 Monitoring

### Local Logs
```bash
# Console logs
dotnet run --project services/Zap.Identity.Api/Zap.Identity.Api.csproj

# File logs (if configured)
cat logs/app.log
```

### GCP Cloud Run Logs
```bash
gcloud run services logs read zap-identity-api --region asia-southeast1
```

### Metrics
- **GCP Console:** https://console.cloud.google.com/run
- **Application Insights:** (if configured)

---

## 🔧 Configuration

### appsettings.json
```json
{
  "DatabaseSettings": {
    "ConnectionString": "mongodb://...",
    "DatabaseName": "SinglePoint_en"
  },
  "JwtSettings": {
    "Secret": "your-secret-key",
    "Issuer": "Zap.Identity.Api",
    "ExpirationInMinutes": 60
  }
}
```

### Environment Variables
```bash
# Development
export ASPNETCORE_ENVIRONMENT=Development

# Production
export ASPNETCORE_ENVIRONMENT=Production
export DatabaseSettings__ConnectionString="mongodb+srv://..."
```

### User Secrets (Development)
```bash
dotnet user-secrets set "DatabaseSettings:ConnectionString" "mongodb://..."
dotnet user-secrets set "JwtSettings:Secret" "your-secret"
```

---

## 🤝 Contributing

### Workflow
1. Create feature branch: `git checkout -b feature/your-feature`
2. Make changes
3. Commit: `git commit -m "feat: your feature"`
4. Push: `git push origin feature/your-feature`
5. Create Pull Request

### Commit Convention
- `feat:` New feature
- `fix:` Bug fix
- `docs:` Documentation
- `refactor:` Code refactoring
- `test:` Tests
- `chore:` Maintenance

### Pull Request Template
Sử dụng template trong [PR_DESCRIPTION.md](PR_DESCRIPTION.md)

---

## 📞 Support

### Documentation
- **This Index:** [DOCUMENTATION_INDEX.md](DOCUMENTATION_INDEX.md)
- **MongoDB Atlas:** [MONGODB_ATLAS_SETUP.md](MONGODB_ATLAS_SETUP.md)
- **GCP Deployment:** [GCP_DEPLOYMENT_DETAILED.md](GCP_DEPLOYMENT_DETAILED.md)

### External Resources
- **.NET Docs:** https://docs.microsoft.com/en-us/dotnet/
- **MongoDB Docs:** https://docs.mongodb.com/
- **Cloud Run Docs:** https://cloud.google.com/run/docs

### Team Contact
- **Email:** dev@zap.vn
- **Slack:** #backend-api
- **GitHub Issues:** https://github.com/ZAP-vn/Developer-Backend-API/issues

---

## 📝 License

[Add license information]

---

## 🎯 Roadmap

### Completed ✅
- [x] MongoDB aggregation fix
- [x] Dynamic data access API
- [x] GCP Cloud Run deployment scripts
- [x] Comprehensive documentation

### In Progress 🚧
- [ ] Unit tests coverage
- [ ] API documentation (Swagger)
- [ ] Performance optimization

### Planned 📅
- [ ] GraphQL support
- [ ] Redis caching
- [ ] Microservices architecture
- [ ] Kubernetes deployment

---

## 📈 Version History

### v1.1.0 (2026-02-12)
- Fix MongoDB aggregation $ifNull error
- Add DynamicRepository for generic data access
- Add GCP Cloud Run deployment support
- Add comprehensive documentation

### v1.0.0 (2026-02-07)
- Initial release
- Basic CRUD operations
- JWT authentication
- MongoDB integration

---

**Last Updated:** 2026-02-12  
**Maintainer:** Zap Development Team
