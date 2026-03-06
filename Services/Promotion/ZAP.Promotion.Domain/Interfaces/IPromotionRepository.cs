using ZAP.BuildingBlocks.Interfaces;
using ZAP.Promotion.Domain.Entities;

namespace ZAP.Promotion.Domain.Interfaces
{
    public interface IPromotionRepository : IMongoRepository<PromotionEntity>
    {
    }
}
