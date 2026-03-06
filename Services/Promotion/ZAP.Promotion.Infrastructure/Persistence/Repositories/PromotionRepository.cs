using MongoDB.Driver;
using ZAP.BuildingBlocks.Interfaces;
using ZAP.BuildingBlocks.Repositories;
using ZAP.Promotion.Domain.Entities;
using ZAP.Promotion.Domain.Interfaces;

namespace ZAP.Promotion.Infrastructure.Persistence.Repositories
{
    public class PromotionRepository : BaseMongoRepository<PromotionEntity>, IPromotionRepository
    {
        public PromotionRepository(IMongoDatabase database, ICurrentUserService currentUserService) 
            : base(database, "Promotions", currentUserService)
        {
        }
    }
}
