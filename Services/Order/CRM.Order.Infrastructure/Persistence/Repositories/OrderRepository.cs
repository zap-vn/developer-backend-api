using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using CRM.BuildingBlocks.Interfaces;
using CRM.BuildingBlocks.Repositories;
using CRM.Order.Domain.Entities;
using CRM.Order.Domain.Interfaces;

namespace CRM.Order.Infrastructure.Persistence.Repositories
{
    public class OrderRepository : BaseMongoRepository<OrderEntity>, IOrderRepository
    {
        public OrderRepository(IMongoDatabase database, ICurrentUserService currentUserService) 
            : base(database, "Orders", currentUserService)
        {
        }

        public async Task<IEnumerable<OrderEntity>> GetByStatusAsync(string status)
        {
            return await FindAsync(x => x.Status == status);
        }

        public async Task<object> GetOrderSummaryAsync(string status, int page, int pageSize)
        {
            var filter = ApplyTenantFilter(x => x.Status == status);
            
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
