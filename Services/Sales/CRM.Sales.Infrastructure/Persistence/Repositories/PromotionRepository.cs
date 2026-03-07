using MongoDB.Driver;
using CRM.BuildingBlocks.Interfaces;
using CRM.BuildingBlocks.Repositories;
using CRM.Sales.Domain.Entities.Promotions;
using CRM.Sales.Domain.Interfaces;

namespace CRM.Sales.Infrastructure.Persistence.Repositories
{
    public class PromotionRepository : BaseMongoRepository<Promotion>, IPromotionRepository
    {
        public PromotionRepository(IMongoDatabase database, ICurrentUserService currentUserService) 
            : base(database, "Promotions", currentUserService)
        {
        }
    }
}
