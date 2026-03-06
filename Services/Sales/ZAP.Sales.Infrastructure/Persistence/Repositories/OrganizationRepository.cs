using MongoDB.Driver;
using ZAP.BuildingBlocks.Interfaces;
using ZAP.BuildingBlocks.Repositories;
using ZAP.Sales.Domain.Entities.Organizations;
using ZAP.Sales.Domain.Interfaces;

namespace ZAP.Sales.Infrastructure.Persistence.Repositories
{
    public class OrganizationRepository : BaseMongoRepository<OrganizationUnit>, IOrganizationRepository
    {
        public OrganizationRepository(IMongoDatabase database, ICurrentUserService currentUserService) 
            : base(database, "OrganizationUnits", currentUserService)
        {
        }
    }
}
