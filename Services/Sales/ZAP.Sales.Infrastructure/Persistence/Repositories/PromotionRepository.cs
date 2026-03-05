using ZAP.BuildingBlocks.Interfaces;
using ZAP.BuildingBlocks.Repositories;
using ZAP.Sales.Domain.Entities;
using ZAP.Sales.Domain.Interfaces;

namespace ZAP.Sales.Infrastructure.Persistence.Repositories
{
    public class PromotionRepository : BaseMongoRepository<Promotion>, IPromotionRepository
    {
        public PromotionRepository(MongoDbContext context, ICurrentUserService currentUserService) 
            : base(context.Database, "Promotions", currentUserService)
        {
        }
    }
}
