using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZAP.BuildingBlocks.Interfaces;
using ZAP.BuildingBlocks.Repositories;
using ZAP.Sales.Domain.Entities.Orders;
using ZAP.Sales.Domain.Interfaces;

namespace ZAP.Sales.Infrastructure.Persistence.Repositories
{
    public class OrderRepository : BaseMongoRepository<OrderEntity>, IOrderRepository
    {
        public OrderRepository(IMongoDatabase database, ICurrentUserService currentUserService) 
            : base(database, "Orders", currentUserService)
        {
        }

        public async Task<IEnumerable<OrderEntity>> GetByStatusAsync(string status)
        {
            // findAsync đã được ApplyTenantFilter tự động
            return await FindAsync(x => x.Status == status);
        }

        /// <summary>
        /// Mẫu Skill: Truy vấn tối ưu cho hàng triệu dòng dữ liệu (Projection)
        /// </summary>
        public async Task<object> GetOrderSummaryAsync(string status, int page, int pageSize)
        {
            var filter = ApplyTenantFilter(x => x.Status == status);
            
            // Chỉ lấy những trường cần thiết để tiết kiệm RAM và Băng thông
            var projection = Builders<OrderEntity>.Projection
                .Include(x => x.OrderCode)
                .Include(x => x.TotalAmount)
                .Include(x => x.CreatedAt);

            var totalItems = await _collection.CountDocumentsAsync(filter);
            
            var items = await _collection.Find(filter)
                .Project(projection)
                .SortByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            return new { Total = totalItems, Data = items };
        }
    }
}
