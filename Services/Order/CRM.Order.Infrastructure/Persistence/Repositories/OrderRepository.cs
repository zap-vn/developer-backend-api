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
        private readonly IMongoDatabase _database;

        public OrderRepository(IMongoDatabase database, ICurrentUserService currentUserService) 
            : base(database, "ordering.Orders", currentUserService)
        {
            _database = database;
        }

        public async Task<IEnumerable<OrderEntity>> GetByStatusAsync(string status)
        {
            if (int.TryParse(status, out int statusId))
            {
                return await FindAsync(x => x.OrderStatusId == statusId);
            }
            return new List<OrderEntity>();
        }

        public async Task<object> GetOrderSummaryAsync(string status, int page, int pageSize)
        {
            int.TryParse(status, out int statusId);
            var filter = ApplyTenantFilter(x => x.OrderStatusId == statusId);
            
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

        public async Task<(List<OrderEntity> Items, long TotalCount)> GetOrderListAsync(
            string? keyword, string? status, int pageIndex, int pageSize)
        {
            var builder    = Builders<OrderEntity>.Filter;
            var conditions = builder.Ne(x => x.IsDeleted, true);

            // Apply tenant filter (UserGuid / EmpGuid) - Temporarily disabled for testing
            // conditions = ApplyTenantFilter(conditions);

            // Keyword search on OrderCode (case-insensitive)
            if (!string.IsNullOrWhiteSpace(keyword))
                conditions &= builder.Regex(x => x.OrderCode,
                    new MongoDB.Bson.BsonRegularExpression(keyword, "i"));

            // Status filter
            if (!string.IsNullOrWhiteSpace(status) && int.TryParse(status, out int statusIdValue))
                conditions &= builder.Eq(x => x.OrderStatusId, statusIdValue);

            var totalCount = await _collection.CountDocumentsAsync(conditions);

            var items = await _collection
                .Find(conditions)
                .SortByDescending(x => x.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<IReadOnlyList<OrderDetailEntity>> GetRelatedOrderDetailsAsync(IEnumerable<string> orderIds)
        {
            var collection = _database.GetCollection<OrderDetailEntity>("ordering.OrderDetail");
            var filter = Builders<OrderDetailEntity>.Filter.In(x => x.OrderId, orderIds);
            return await collection.Find(filter).ToListAsync();
        }

        public async Task<IReadOnlyList<LocationEntity>> GetLocationsAsync(IEnumerable<string> locationGuids)
        {
            var collection = _database.GetCollection<LocationEntity>("ordering.Location");
            var filter = Builders<LocationEntity>.Filter.In(x => x.Id, locationGuids);
            return await collection.Find(filter).ToListAsync();
        }
    }
}
