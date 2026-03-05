using ZAP.BuildingBlocks.Interfaces;
using ZAP.BuildingBlocks.Repositories;
using ZAP.Organization.Domain.Entities;
using ZAP.Organization.Domain.Interfaces;

namespace ZAP.Organization.Infrastructure.Persistence.Repositories
{
    public class OrganizationRepository : BaseMongoRepository<OrganizationUnit>, IOrganizationRepository
    {
        public OrganizationRepository(MongoDbContext context, ICurrentUserService currentUserService) 
            : base(context.Database, "OrganizationUnits", currentUserService)
        {
        }
    }
}
