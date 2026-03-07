using CRM.BuildingBlocks.Interfaces;
using CRM.Promotion.Domain.Entities;

namespace CRM.Promotion.Domain.Interfaces
{
    public interface IPromotionRepository : IMongoRepository<PromotionEntity>
    {
    }
}
