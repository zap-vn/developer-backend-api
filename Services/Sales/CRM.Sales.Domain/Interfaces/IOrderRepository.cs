using System.Collections.Generic;
using System.Threading.Tasks;
using CRM.BuildingBlocks.Interfaces;
using CRM.Sales.Domain.Entities.Orders;

namespace CRM.Sales.Domain.Interfaces
{
    public interface IOrderRepository : IMongoRepository<OrderEntity>
    {
        Task<IEnumerable<OrderEntity>> GetByStatusAsync(string status);
        Task<object> GetOrderSummaryAsync(string status, int page, int pageSize);
    }
}
