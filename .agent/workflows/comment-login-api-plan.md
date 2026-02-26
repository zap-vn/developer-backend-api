---
description: Chi tiết kế hoạch triển khai cụm API Comment và Login (Stack: C#, MongoDB, JWT)
---

# Skill: Kế hoạch Triển khai API Comment + Login

Tài liệu này tóm tắt chi tiết cấu trúc và các bước thực hiện cho dự án API Comment & Login sử dụng **ASP.NET Core**, **MongoDB**, và **JWT**.

## 1. Công nghệ sử dụng (Stack)
- **Backend:** C# (ASP.NET Core 8)
- **Database:** MongoDB
- **Authentication:** JWT (JSON Web Token)
- **Password Security:** BCrypt (Hỗ trợ cả legacy hash MD5/SHA256)

## 2. Các thành phần chính (Core Modules)

### A. Auth (Xác thực & Người dùng)
- **Đăng ký (Register):** Tạo mới User, mật khẩu sẽ được hash bằng **BCrypt** trước khi lưu vào MongoDB.
- **Đăng nhập (Login):** Kiểm tra thông tin User + MerchantName. Nếu hợp lệ, trả về JWT Token và các thông tin cơ bản (FullName, Avatar).
- **Profile:** Xem thông tin chi tiết user từ Token đã đăng nhập.

### B. Comment (Bình luận & Tương tác)
- **CRUD:**
  - `GET /api/posts/{postId}/comments`: Xem danh sách bình luận (Public - Ai cũng xem được).
  - `POST /api/posts/{postId}/comments`: Tạo bình luận (Yêu cầu đăng nhập).
  - `PUT /api/comments/{id}`: Sửa bình luận (Chính chủ mới được sửa).
  - `DELETE /api/comments/{id}`: Xóa bình luận (Chính chủ hoặc Admin/Owner).
- **Reply lồng nhau (Nested):** Hỗ trợ `ParentId` để tạo các luồng trả lời trực tiếp cho một bình luận.

### C. Infrastructure (Hạ tầng & Hệ thống)
- **MongoDB Persistence:** Quản lý kết nối, bộ đếm ID tự tăng (`ManagementIndex`) và các truy vấn tối ưu.
- **JWT Middleware:** Xử lý xác thực request từ Header, trích xuất `UserGuid` và `RoleName`.
- **Validation DTOs:** Sử dụng Data Annotations để validate dữ liệu đầu vào (Email, Password không được trống, v.v.).

## 3. Lộ trình triển khai (Work Roadmap)

1.  **Setup project:** Khởi tạo project, cài đặt các package MongoDB, JWT, BCrypt. Cấu hình Connection String trong `appsettings.json`.
2.  **Models (User, Comment):** Định nghĩa các class Entity trong lớp Domain và các Map tương ứng cho MongoDB.
3.  **JWT Service:** Viết helper tạo Token và cấu hình Authentication middleware trong `Program.cs`.
4.  **Auth API:** Triển khai `AuthService` và `AuthController` (Login/Register).
5.  **Comment API:** Triển khai `CommentService` và `CommentsController` (CRUD/Reply).
6.  **Test Postman:** Thực hiện các kịch bản test: Đăng ký -> Đăng nhập -> Lấy Token -> Tạo Comment -> Reply -> Xóa.

---
> [!NOTE]
> Skill này được thiết kế để áp dụng cho cấu trúc Clean Architecture của Zap.Backend.
