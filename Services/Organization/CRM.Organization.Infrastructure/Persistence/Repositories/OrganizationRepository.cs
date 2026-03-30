using MongoDB.Driver;
using CRM.BuildingBlocks.Interfaces;
using CRM.BuildingBlocks.Repositories;
using CRM.Organization.Domain.Entities;
using CRM.Organization.Domain.Interfaces;

namespace CRM.Organization.Infrastructure.Persistence.Repositories
{
    public class OrganizationRepository : BaseMongoRepository<OrganizationUnit>, IOrganizationRepository
    {
        public OrganizationRepository(IMongoDatabase database, ICurrentUserService currentUserService) 
            : base(database, "merchant.OrganizationUnits", currentUserService)
        {
        }
    }
}
