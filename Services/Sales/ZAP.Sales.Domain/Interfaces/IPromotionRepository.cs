using ZAP.BuildingBlocks.Interfaces;
using ZAP.Sales.Domain.Entities;

namespace ZAP.Sales.Domain.Interfaces
{
    public interface IPromotionRepository : IMongoRepository<Promotion>
    {
    }
}
