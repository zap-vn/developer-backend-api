using System.Collections.Generic;
using System.Threading.Tasks;
using ZAP.BuildingBlocks.Interfaces;
using ZAP.Order.Domain.Entities;

namespace ZAP.Order.Domain.Interfaces
{
    public interface IOrderRepository : IMongoRepository<OrderEntity>
    {
        // Thêm các phương thức đặc thù cho Order nếu cần
        Task<IEnumerable<OrderEntity>> GetByStatusAsync(string status);
    }
}
