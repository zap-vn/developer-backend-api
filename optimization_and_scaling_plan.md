# CHIẾN LƯỢC TỐI ƯU HÓA HỆ THỐNG QUY MÔ LỚN (HIGH-SCALE OPTIMIZATION SKILL)

Phân tích yêu cầu: Hệ thống xử lý dữ liệu cho nhiều Merchant, số lượng Order có thể lên tới hàng triệu dòng. 

## 1. CHIẾN LƯỢC LƯU TRỮ DỮ LIỆU (DATABASE STRATEGY - MONGODB)

### 1.1. Phân vùng dữ liệu (Sharding)
Với quy mô hàng triệu dòng, Sharding là bắt buộc để chia tải.
- **Shard Key:** Sử dụng `UserGuid` làm Shard Key.
- **Lý do:** 
  - Đảm bảo dữ liệu của một Merchant nằm trên cùng một Shard (Locality).
  - Tránh tình trạng "Hot Partition" (một node bị quá tải trong khi node khác rảnh).
  - Giúp các truy vấn theo Merchant (chiếm 90% traffic) đạt hiệu năng cao nhất.

### 1.2. Chiến lược Chỉ mục (Indexing Strategy)
- **Compound Index:** Cần đánh index kết hợp `{ UserGuid: 1, CreatedDate: -1 }` cho các truy vấn danh sách order.
- **Partial Index:** Chỉ đánh index trên các Order đang xử lý (Active Orders) để giảm dung lượng Index trong RAM.
  - *Ví dụ:* `db.Orders.createIndex({ UserGuid: 1 }, { partialFilterExpression: { Status: { $ne: "Completed" } } })`
- **TTL Index:** Tự động xóa hoặc di chuyển dữ liệu log/temp cũ sau một khoảng thời gian (ví dụ 30 ngày).

### 1.3. Localized Data Snapshot (Quan trọng cho Order)
- Đối với bảng Order, **KHÔNG** nên query bản dịch từ bảng Product mỗi khi xem đơn hàng.
- **Giải pháp:** Khi tạo Order, hệ thống sẽ chèn trực tiếp thông tin sản phẩm (bao gồm cả Name đã được dịch tại thời điểm đó) vào trong Document Order. Điều này giúp lịch sử đơn hàng không bị thay đổi khi Product bị cập nhật và tăng tốc độ hiển thị đơn hàng.

## 2. TỐI ƯU HÓA MÃ NGUỒN (CODE OPTIMIZATION - .NET Core)

### 2.1. Sử dụng Projection (Lọc trường dữ liệu)
- Tuyệt đối không dùng `Find().ToList()` cho các bảng lớn. Chỉ lấy những trường cần thiết.
- **Mẫu tối ưu:**
  ```csharp
  var projection = Builders<OrderEntity>.Projection
      .Include(x => x.OrderCode)
      .Include(x => x.TotalAmount);
  var data = await _collection.Find(x => x.UserGuid == userGuid)
      .Project<OrderShortDto>(projection) // Chuyển trực tiếp sang DTO tại DB
      .Limit(50)
      .ToListAsync();
  ```

### 2.2. Xử lý Batch & Async Stream
- Khi cần xử lý hoặc xuất báo cáo lớn, sử dụng `IAsyncEnumerable` để tránh tràn bộ nhớ (OutOfMemory Exception).
- Sử dụng `BulkWriteAsync` cho các tác vụ cập nhật hàng loạt (Update Status, Sync tồn kho).

### 2.3. Caching Layer (Redis)
- **Master Data (Product, Merchant Settings):** Cache vào Redis với key dạng `merchant:{id}:products`. 
- **Localization Cache:** Cache các bản dịch phổ biến để không cần truy cập MongoDB cho mỗi request.

## 3. THIẾT KẾ ĐA THUÊ (MULTI-TENANCY REFACTORING)

Chúng ta cần cập nhật tất cả Entity để hỗ trợ lọc theo Merchant:

### 3.1. Cấu trúc Base Entity mới
```csharp
public abstract class BaseTenantEntity : BaseEntity 
{
    public Guid MerchantId { get; set; } // Trường bắt buộc để phân vùng dữ liệu
}
```

### 3.2. Global Query Filter
Tự động thêm điều kiện `x => x.UserGuid == currentUserGuid` vào tất cả các truy vấn thông qua Repository Base để tránh việc developer quên lọc dữ liệu, dẫn đến rò rò rỉ dữ liệu giữa các Merchant.

## 4. KẾ HOẠCH HÀNH ĐỘNG (ACTION PLAN)

- [ ] **Giai đoạn 1: Chuẩn hóa Schema**
  - Thêm `UserGuid` vào tất cả các bảng (Products, Orders, Employees).
  - Tạo Index tương ứng trên MongoDB.
- [ ] **Giai đoạn 2: Tối ưu Repository**
  - Chuyển đổi các hàm `GetList` sang dạng hỗ trợ Phân trang (Pagination) và Projection.
- [ ] **Giai đoạn 3: Triển khai Redis**
  - Cài đặt Redis và cấu hình Cache cho các thông tin ít thay đổi nhưng hay truy cập.
- [ ] **Giai đoạn 4: Monitoring**
  - Cài đặt các công cụ theo dõi Slow Queries trên MongoDB để tối ưu kịp thời.
