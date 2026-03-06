using System.Collections.Generic;
using System.Threading.Tasks;
using ZAP.BuildingBlocks.Interfaces;
using ZAP.Order.Domain.Entities;

namespace ZAP.Order.Domain.Interfaces
{
    public interface IOrderRepository : IMongoRepository<OrderEntity>
    {
        Task<IEnumerable<OrderEntity>> GetByStatusAsync(string status);
        Task<object> GetOrderSummaryAsync(string status, int page, int pageSize);
    }
}
