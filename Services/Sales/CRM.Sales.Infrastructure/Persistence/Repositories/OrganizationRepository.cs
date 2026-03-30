using MongoDB.Driver;
using CRM.BuildingBlocks.Interfaces;
using CRM.BuildingBlocks.Repositories;
using CRM.Sales.Domain.Entities.Organizations;
using CRM.Sales.Domain.Interfaces;

namespace CRM.Sales.Infrastructure.Persistence.Repositories
{
    public class OrganizationRepository : BaseMongoRepository<OrganizationUnit>, IOrganizationRepository
    {
        public OrganizationRepository(IMongoDatabase database, ICurrentUserService currentUserService) 
            : base(database, "merchant.OrganizationUnits", currentUserService)
        {
        }
    }
}
