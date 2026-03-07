using CRM.BuildingBlocks.Interfaces;
using CRM.Sales.Domain.Entities.Promotions;

namespace CRM.Sales.Domain.Interfaces
{
    public interface IPromotionRepository : IMongoRepository<Promotion>
    {
    }
}
