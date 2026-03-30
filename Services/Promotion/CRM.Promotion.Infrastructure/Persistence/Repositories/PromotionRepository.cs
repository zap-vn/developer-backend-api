using MongoDB.Driver;
using CRM.BuildingBlocks.Interfaces;
using CRM.BuildingBlocks.Repositories;
using CRM.Promotion.Domain.Entities;
using CRM.Promotion.Domain.Interfaces;

namespace CRM.Promotion.Infrastructure.Persistence.Repositories
{
    public class PromotionRepository : BaseMongoRepository<PromotionEntity>, IPromotionRepository
    {
        public PromotionRepository(IMongoDatabase database, ICurrentUserService currentUserService) 
            : base(database, "promotion.Promotions", currentUserService)
        {
        }
    }
}
