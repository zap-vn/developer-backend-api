---
description: Quy trình chuẩn để triển khai API mới trong hệ thống Zap.Backend (ví dụ: Comment, Login)
---

# Kế hoạch & Quy trình triển khai API (API Implementation Skill)

Tài liệu này hướng dẫn các bước chi tiết để thêm một tính năng API mới vào hệ thống Zap.Backend, tuân thủ kiến trúc Clean Architecture hiện tại.

## Bước 1: Định nghĩa Domain (Domain Layer)
1. Xác định Entity cần thiết trong thư mục `services/Zap.Identity.Domain/Entities`.
2. Đảm bảo Entity có các thuộc tính:
   - `Id` (Kiểu string, mapping với MongoDB `_id`).
   - `Visible` (1 là hiện, 0 là ẩn/xóa mềm).
   - `CreateDate`, `UpdateDate` (String ISO 8601).
   - Các thuộc tính nghiệp vụ (ví dụ: `Content`, `AuthorId`, `PostId`).

## Bước 2: Định nghĩa DTO & Interface (Application Layer)
1. Tạo DTO trong `services/Zap.Identity.Application/DTOs`.
   - Sử dụng `[JsonPropertyName("_id")]` cho field Id.
   - Thêm các field phụ để tiện cho UI (ví dụ: `AuthorName`, `AuthorAvatar`).
2. Khai báo Interface Service (ví dụ: `ICommentService`) trong `services/Zap.Identity.Application/Interfaces`.

## Bước 3: Triển khai Repository (Infrastructure Layer)
1. Cập nhật hoặc tạo Repository trong `services/Zap.Identity.Infrastructure/Repositories`.
2. Xử lý logic Auto-Increment (nếu cần) thông qua `ManagementIndex` collection:
   ```csharp
   var filter = Builders<BsonDocument>.Filter.Eq("_id", "{Entity}_id");
   var update = Builders<BsonDocument>.Update.Inc("Value", 1)...
   ```
3. Implement các hàm Get, Create, Update, Delete (Xóa mềm bằng cách set `Visible = 0`).

## Bước 4: Triển khai Service & Logic (Infrastructure Layer)
1. Tạo Service implementation trong `services/Zap.Identity.Infrastructure/Services`.
2. **Logic Check quyền**:
   - Chỉ cho phép chỉnh sửa nếu `AuthorId == CurrentUserGuid`.
3. **Logic Join dữ liệu**:
   - Nếu cần thông tin người dùng (tên, avatar), hãy inject `ICustomerRepository` và dùng `GetByIdsAsync` để populate dữ liệu vào DTO.
4. **Logic Login/Common**:
   - Đối với Login, sử dụng `AuthService` để verify password (legacy hash + BCrypt) và generate JWT với các claims chuẩn (`UserGuid`, `RoleName`).

## Bước 5: Tạo Controller (Api Layer)
1. Tạo Controller trong `services/Zap.Identity.Api/Controllers`, kế thừa từ `BaseApiController`.
2. Sử dụng `[Authorize]` mặc định.
3. Access thông tin user qua `CurrentUserGuid` và `IsAdmin` properties trong `BaseApiController`.
4. Định nghĩa Route chuẩn: `api/[controller]` hoặc các route chuyên biệt như `api/posts/{postId}/comments`.

## Bước 6: Cấu hình Gateway (Root Folder)
1. Cập nhật file `api-gateway.yaml`.
2. Khai báo các endpoint mới và trỏ về Cloud Run backend address.
3. Đảm bảo định nghĩa đúng `operationId` và `path_translation`.

## Bước 7: Kiểm tra & Build
1. Build project: `dotnet build services/Zap.Identity.Api/Zap.Identity.Api.csproj`.
## Phụ lục: Kế hoạch mẫu (Comment + Login API)

**Stack:** C# (ASP.NET Core) + MongoDB + JWT

### Các phần hành chính:
1. **Auth:** Đăng ký, đăng nhập trả về JWT token, xem thông tin user. Mật khẩu hash bằng **BCrypt**.
2. **Comment:** CRUD comment theo bài post, hỗ trợ reply lồng nhau (ParentId). Các thao tác tạo/sửa/xóa yêu cầu đăng nhập và kiểm tra quyền (Chính chủ/Admin).
3. **Infrastructure:** MongoDB lưu trữ, JWT middleware xác thực request, DTOs validate input.

### Thứ tự thực hiện (Roadmap):
1. **Setup project**: Cấu hình MongoDB và JWT trong `Program.cs`.
2. **Models (User, Comment)**: Định nghĩa Entity và Repository.
3. **JWT Service**: Triển khai logic tạo token trong `AuthService`.
4. **Auth API**: Viết các endpoint Login/Register.
5. **Comment API**: Viết các endpoint CRUD và logic lồng nhau.
6. **Test Postman**: Chạy qua Gateway để kiểm tra thực tế.
