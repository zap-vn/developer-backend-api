using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using CRM.BuildingBlocks.Interfaces;
using CRM.BuildingBlocks.Repositories;
using CRM.Sales.Domain.Entities.Orders;
using CRM.Sales.Domain.Interfaces;

namespace CRM.Sales.Infrastructure.Persistence.Repositories
{
    public class OrderRepository : BaseMongoRepository<OrderEntity>, IOrderRepository
    {
        public OrderRepository(IMongoDatabase database, ICurrentUserService currentUserService) 
            : base(database, "ordering.Orders", currentUserService)
        {
        }

        public async Task<IEnumerable<OrderEntity>> GetByStatusAsync(string status)
        {
            // findAsync has been automatically ApplyTenantFilter
            return await FindAsync(x => x.Status == status);
        }

        /// <summary>
        /// Skill Template: Optimized query for millions of rows (Projection)
        /// </summary>
        public async Task<object> GetOrderSummaryAsync(string status, int page, int pageSize)
        {
            var filter = ApplyTenantFilter(x => x.Status == status);
            
            // Only fetch necessary fields to save RAM and Bandwidth
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
