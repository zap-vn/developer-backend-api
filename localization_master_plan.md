# CHIẾN LƯỢC TRIỂN KHAI ĐA NGÔN NGỮ CHI TIẾT (LOCALIZATION MASTER SKILL PLAN)

Dựa trên phân tích hệ thống hiện tại tại Server MongoDB `172.16.10.153` và cấu trúc mã nguồn .NET Core, đây là kế hoạch chi tiết để triển khai tính năng đa ngôn ngữ đồng bộ cho toàn bộ hệ thống CRM.

## 1. PHÂN TÍCH KIẾN TRÚC (ARCHITECTURAL ANALYSIS)

### 1.1. Pattern: Embedded Translation Documents
Chúng ta sẽ sử dụng pattern **Embedded Documents** (Tài liệu lồng nhau) trong MongoDB. 
- **Lý do:** Tối ưu hóa việc đọc dữ liệu (Read-heavy). Chỉ cần 1 truy vấn để lấy toàn bộ thông tin gốc và các bản dịch. MongoDB hỗ trợ query hiệu quả trong mảng (Sub-document queries).

### 1.2. Cấu trúc Core (Building Blocks)
Hệ thống đã có sẵn nền tảng:
- `BaseTranslationEntity`: Chứa `LanguageCode` và `EntityId`.
- `ILocalizable<T>`: Giao diện ràng buộc Main Entity phải có danh sách `Translations`.
- `LocalizationService`: Cung cấp ngôn ngữ hiện tại của Request.

## 2. KẾ HOẠCH TRIỂN KHAI CHI TIẾT THEO TỪNG SERVICE (SERVICE INVENTORY)

| Service | Collection/Entity | Trường cần dịch (Fields) | Trạng thái |
| :--- | :--- | :--- | :--- |
| **Product** | `Product` | Name, Description | Đã xong Domain |
| **HR** | `Employee` | FirstName, LastName, Position, Department | Đã có Entity dịch, chưa link |
| **Organization** | `OrganizationUnit` | Name, Description, Address | Cần tạo mới |
| **Customer** | `CustomerGroup` | Name, Note | Cần tạo mới |
| **Sales** | `Promotion` | Title, Description, Terms | Cần tạo mới |
| **Order** | `ShippingMethod` | Name, Description | Cần tạo mới |

## 3. QUY TRÌNH THỰC HIỆN "SKILL" (STEP-BY-STEP WORKFLOW)

### Giai đoạn 1: Chuẩn hóa Domain (Domain Layer)
1. **Tạo Translation Entity**: 
   - Kế thừa `BaseTranslationEntity`.
   - Chứa các trường cần dịch (String).
2. **Cập nhật Main Entity**:
   - Kể thừa `ILocalizable<TTranslation>`.
   - Thêm `public ICollection<TTranslation> Translations { get; set; } = new List<TTranslation>();`.
   - Giữ các trường gốc làm giá trị mặc định (Fallback - Tiếng Việt).

### Giai đoạn 2: Cơ sở dữ liệu (Infrastructure Layer)
1. **Mapping MongoDB**:
   - Tự động hóa việc map class thông qua `BsonClassMap` nếu cần (thường MongoDB Driver tự xử lý được mảng lồng nhau).
2. **Repository Update**:
   - Viết các extension method để Query bản dịch nhanh chóng.

### Giai đoạn 3: Ứng dụng & Hiển thị (Application Layer)
1. **Localization DTO**: Tạo DTO chứa dữ liệu đã được dịch.
2. **Mapping Profile (AutoMapper)**:
   - Cấu hình mapping logic: `dest.Name = src.Translations.FirstOrDefault(x => x.LanguageCode == currentLang)?.Name ?? src.Name;`.
3. **Application Service**: Inject `ILocalizationService` để lấy ngôn ngữ hiện tại từ Header.

## 4. QUY TẮC DỮ LIỆU (DATA RULES)
- **Mặc định (Fallback)**: Nếu không tìm thấy ngôn ngữ yêu cầu, hệ thống PHẢI lấy dữ liệu ở các trường gốc (thường là Tiếng Việt).
- **ID nhất quán**: `EntityId` trong `Translation` phải khớp với `Id` của `Main Entity`.
- **ISO Standard**: Sử dụng mã chuẩn như `vi-VN`, `en-US`, `ja-JP`.

## 5. DANH SÁCH CÔNG VIỆC CẦN LÀM NGAY (ACTION ITEMS)

- [ ] **Item 1: Update HR Service**
  - Chỉnh sửa `Employee.cs` để kế thừa `ILocalizable<EmployeeTranslation>`.
- [ ] **Item 2: Implement Organization Service**
  - Tạo `OrganizationUnitEntity.cs` và `OrganizationUnitTranslation.cs`.
- [ ] **Item 3: Create Localization Middleware Global**
  - Đảm bảo tất cả các Service đều dùng chung `LocalizationService` từ `BuildingBlocks`.
- [ ] **Item 4: Data Migration Script**
  - Viết file `.js` chạy trên MongoDB để chuyển dữ liệu cũ vào mảng `Translations` mới.

## 6. MẪU TRUY VẤN MONGODB (QUERY EXAMPLE)
```javascript
// Query bản dịch tiếng Anh cho sản phẩm
db.Products.find({
  "Translations.LanguageCode": "en-US"
}, {
  "Name": 1, 
  "Translations.$": 1
})
```

---
*Kế hoạch này sẽ được cập nhật liên tục dựa trên tiến độ thực tế.*
