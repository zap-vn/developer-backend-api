using MongoDB.Driver;
using ZAP.BuildingBlocks.Interfaces;
using ZAP.BuildingBlocks.Repositories;
using ZAP.Sales.Domain.Entities.Promotions;
using ZAP.Sales.Domain.Interfaces;

namespace ZAP.Sales.Infrastructure.Persistence.Repositories
{
    public class PromotionRepository : BaseMongoRepository<Promotion>, IPromotionRepository
    {
        public PromotionRepository(IMongoDatabase database, ICurrentUserService currentUserService) 
            : base(database, "Promotions", currentUserService)
        {
        }
    }
}
