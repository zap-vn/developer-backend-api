# Kế hoạch Triển khai Đa ngôn ngữ (Localization Skill Plan)

## 1. Mục tiêu
Triển khai hệ thống đa ngôn ngữ đồng nhất cho toàn bộ các dịch vụ (Services) trong dự án ZAP. Hệ thống sẽ sử dụng cơ chế lưu trữ "Bảng gốc" (Main Entity) và "Bảng dịch" (Translation Entity) theo từng tính năng hoặc Collection.

## 2. Thông tin môi trường
- **Cơ sở dữ liệu:** MongoDB
- **Connection String:** `mongodb://172.16.10.153:27017/?retryWrites=false&loadBalanced=false&connectTimeoutMS=10000`
- **Ngôn ngữ hỗ trợ mặc định:** `vi-VN` (Tiếng Việt)
- **Các ngôn ngữ khác:** `en-US` (Tiếng Anh), `de-DE` (Tiếng Đức), `ja-JP` (Tiếng Nhật).

## 3. Kiến trúc dữ liệu (Pattern)

Dựa trên cấu trúc sẵn có trong `ZAP.BuildingBlocks`, chúng ta sẽ áp dụng pattern sau cho tất cả các Collection cần đa ngôn ngữ:

### A. Thực thể Dịch (Translation Entity)
Tất cả các thực thể dịch phải kế thừa từ `BaseTranslationEntity`.
- Ví dụ cho Sản phẩm: `ProductTranslation` chứa các trường cần dịch như `Name`, `Description`.

### B. Thực thể Gốc (Main Entity)
Thực thể gốc sẽ chứa:
1. Các trường dữ liệu chung (Price, Code, Stock, v.v.)
2. Giá trị mặc định (thường là tiếng Việt) để hỗ trợ fallback.
3. Danh sách các bản dịch: `ICollection<TTranslation> Translations`.

### C. Giao diện (Interface)
Sử dụng `ILocalizable<T>` để đảm bảo tính nhất quán giữa các Domain.

## 4. Danh sách các bước triển khai (Workflow)

### Bước 1: Xác định các Collection cần đa ngôn ngữ
Rà soát trong tất cả các Services (Product, HR, Organization, Customer, Sales, Order, v.v.) các bảng cần hỗ trợ nhiều ngôn ngữ:
- [x] **Product**: `ProductEntity`, `ProductTranslation` (Đã thực hiện)
- [x] **HR**: `EmployeeEntity`, `EmployeeTranslation` (Đã sơ bộ)
- [ ] **Organization**: Đơn vị công tác, Phòng ban.
- [ ] **Sales/Promotion**: Tên chương trình khuyến mãi.
- [ ] **Customer**: Loại khách hàng.

### Bước 2: Tạo các Class Entity
Tại mỗi Domain Service:
1. Tạo file `[EntityName]Translation.cs` trong thư mục `Entities`.
2. Cập nhật `[EntityName].cs` kế thừa `ILocalizable` và thêm property `Translations`.

### Bước 3: Cập nhật Repositories và Unit of Work
Đảm bảo khi truy vấn dữ liệu, hệ thống có thể:
1. Load kèm các bản dịch liên quan.
2. Hỗ trợ Filter/Sort theo ngôn ngữ hiện tại nếu cần.

### Bước 4: Tích hợp Middleware Localization
Cấu hồi `LocalizationMiddleware` trong `BuildingBlocks` để tự động nhận diện ngôn ngữ từ Header `Accept-Language` của Request.

### Bước 5: Cập nhật DTOs và Mapping
Sử dụng AutoMapper hoặc Manual Mapping để trả về dữ liệu đã được "lọc" theo ngôn ngữ của người dùng:
- Nếu có bản dịch cho ngôn ngữ yêu cầu -> Trả về bản dịch.
- Nếu không có -> Trả về giá trị mặc định từ thực thể gốc.

## 5. Mẫu Code Chuẩn

```csharp
// 1. Translation Entity
public class CategoryTranslation : BaseTranslationEntity
{
    public string Name { get; set; } = string.Empty;
}

// 2. Main Entity
public class CategoryEntity : BaseEntity, ILocalizable<CategoryTranslation>
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty; // Default (vi-VN)
    public ICollection<CategoryTranslation> Translations { get; set; } = new List<CategoryTranslation>();
}
```

## 6. Kiểm tra và Nghiệm thu
1. Kiểm tra việc lưu trữ trong MongoDB (kiểm tra cấu trúc lồng nhau hoặc bảng riêng).
2. Kiểm tra API Response khi thay đổi Header `Accept-Language`.
3. Kiểm tra tính toàn vẹn dữ liệu khi Update bản dịch.
