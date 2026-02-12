# 🚀 Zap Backend API

Backend API cho Zap Platform - Hệ thống quản lý dữ liệu động với .NET 8 và MongoDB.

[![.NET](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com/)
[![MongoDB](https://img.shields.io/badge/MongoDB-4.4+-green)](https://www.mongodb.com/)
[![Cloud Run](https://img.shields.io/badge/GCP-Cloud%20Run-blue)](https://cloud.google.com/run)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

---

## ✨ Features

- 🔐 **JWT Authentication** - Secure token-based authentication
- 📊 **Dynamic Data Access** - Generic repository pattern for MongoDB collections
- 🌍 **Multi-language Support** - Automatic translation handling
- 🖼️ **Image Management** - Integrated image handling with collections
- 🚀 **Cloud Ready** - Docker & GCP Cloud Run deployment
- 📝 **RESTful API** - Clean and intuitive API design
- ⚡ **High Performance** - Optimized MongoDB aggregation pipelines

---

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────────┐
│                     API Layer                           │
│  (Controllers, Middleware, Authentication)              │
└────────────────┬────────────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────────────┐
│               Application Layer                         │
│  (DTOs, Interfaces, Business Logic)                     │
└────────────────┬────────────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────────────┐
│              Infrastructure Layer                       │
│  (Repositories, Services, MongoDB Integration)          │
└────────────────┬────────────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────────────┐
│                 Domain Layer                            │
│  (Entities, Value Objects)                              │
└─────────────────────────────────────────────────────────┘
```

---

## 🚀 Quick Start

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MongoDB](https://www.mongodb.com/try/download/community) hoặc [MongoDB Atlas](https://www.mongodb.com/cloud/atlas)
- [Docker](https://www.docker.com/) (optional)

### Installation

1. **Clone repository:**
   ```bash
   git clone https://github.com/ZAP-vn/Developer-Backend-API.git
   cd Zap.Backend
   ```

2. **Update connection string:**
   ```bash
   # Edit appsettings.json
   {
     "DatabaseSettings": {
       "ConnectionString": "mongodb://localhost:27017"
     }
   }
   ```

3. **Run API:**
   ```bash
   dotnet run --project services/Zap.Identity.Api/Zap.Identity.Api.csproj
   ```

4. **Access Swagger UI:**
   ```
   http://localhost:5271/swagger
   ```

---

## 📖 Documentation

| Document | Description |
|----------|-------------|
| [📚 Documentation Index](DOCUMENTATION_INDEX.md) | Tổng hợp tất cả documentation |
| [🍃 MongoDB Atlas Setup](MONGODB_ATLAS_SETUP.md) | Hướng dẫn setup MongoDB Atlas |
| [🚀 GCP Deployment](GCP_DEPLOYMENT_DETAILED.md) | Deploy lên Google Cloud Run |
| [⚡ Quick Start GCP](QUICK_START_GCP.md) | Hướng dẫn nhanh deploy GCP |
| [🔐 Authentication Guide](AUTHENTICATION_GUIDE.md) | Hướng dẫn authentication |

---

## 🔌 API Endpoints

### Authentication
```http
POST /api/Auth/login
POST /api/Auth/refresh
```

### Dynamic Data Access
```http
GET    /api/Data/{collection}
GET    /api/Data/{collection}/{id}
POST   /api/Data/{collection}
PUT    /api/Data/{collection}/{id}
DELETE /api/Data/{collection}/{id}
```

### Examples

**Login:**
```bash
curl -X POST http://localhost:5271/api/Auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "UserName": "admin@example.com",
    "Password": "password123",
    "AcceptName": "company",
    "IsRemember": true
  }'
```

**Get Products:**
```bash
curl http://localhost:5271/api/Data/Product \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

---

## 🐛 Recent Fixes

### MongoDB Aggregation Error (v1.1.0)
**Issue:** `Expression $ifNull takes exactly 2 arguments. 4 were passed in`

**Solution:** Fixed in commit [`c8af760`](https://github.com/ZAP-vn/Developer-Backend-API/commit/c8af760)
- Properly nested `$ifNull` operators in aggregation pipeline
- Each `$ifNull` now correctly takes exactly 2 arguments

**Details:** See [PR Description](PR_DESCRIPTION.md)

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
```powershell
# Windows
.\deploy-gcp.ps1

# Linux/Mac
./deploy-gcp.sh
```

**Detailed Guide:** [GCP_DEPLOYMENT_DETAILED.md](GCP_DEPLOYMENT_DETAILED.md)

---

## 🧪 Testing

### Run Tests
```bash
dotnet test
```

### Manual Testing
- **Swagger UI:** http://localhost:5271/swagger
- **Postman:** Import collection from `postman_collection.json`

### Test Scripts
```powershell
# Test login
.\test_login.ps1

# Test with custom token
.\test_my_token.ps1
```

---

## 🔧 Configuration

### appsettings.json
```json
{
  "DatabaseSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "SinglePoint_en",
    "Databases": {
      "Identity": "SinglePoint_en",
      "System": "SinglePoint_System",
      "Orders": "SinglePoint_Orders_vi"
    }
  },
  "JwtSettings": {
    "Secret": "your-secret-key-min-32-characters",
    "Issuer": "Zap.Identity.Api",
    "Audience": "Zap.Client",
    "ExpirationInMinutes": 60
  }
}
```

### Environment Variables
```bash
export ASPNETCORE_ENVIRONMENT=Production
export DatabaseSettings__ConnectionString="mongodb+srv://..."
export JwtSettings__Secret="your-production-secret"
```

---

## 🤝 Contributing

1. Fork the repository
2. Create feature branch: `git checkout -b feature/amazing-feature`
3. Commit changes: `git commit -m 'feat: add amazing feature'`
4. Push to branch: `git push origin feature/amazing-feature`
5. Open Pull Request

**PR Template:** [PR_DESCRIPTION.md](PR_DESCRIPTION.md)

---

## 📊 Project Status

### Current Version: v1.1.0

**Recent Updates:**
- ✅ Fixed MongoDB aggregation $ifNull error
- ✅ Added DynamicRepository for generic data access
- ✅ GCP Cloud Run deployment support
- ✅ Comprehensive documentation

**Roadmap:**
- 🚧 Unit tests coverage improvement
- 📅 GraphQL support
- 📅 Redis caching layer
- 📅 Microservices architecture

---

## 💰 Cost Estimates

### GCP Cloud Run (Free Tier)
- 2 million requests/month
- 360,000 GiB-seconds memory
- 180,000 vCPU-seconds

**Estimated cost:** $0-10/month for small to medium traffic

### MongoDB Atlas (Free Tier)
- 512MB storage
- Shared cluster
- Basic monitoring

**Cost:** $0/month (free forever)

---

## 📞 Support

- **Documentation:** [DOCUMENTATION_INDEX.md](DOCUMENTATION_INDEX.md)
- **Issues:** [GitHub Issues](https://github.com/ZAP-vn/Developer-Backend-API/issues)
- **Email:** dev@zap.vn

---

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 👥 Team

**Zap Development Team**
- Backend API Development
- DevOps & Infrastructure
- Documentation

---

## 🙏 Acknowledgments

- [.NET Team](https://github.com/dotnet) for the amazing framework
- [MongoDB](https://www.mongodb.com/) for the database
- [Google Cloud](https://cloud.google.com/) for Cloud Run platform

---

**⭐ If you find this project useful, please give it a star!**

---

**Last Updated:** 2026-02-12  
**Version:** 1.1.0  
**Branch:** DEV_API_LINH_20260207
