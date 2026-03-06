using ZAP.BuildingBlocks.Interfaces;
using ZAP.Sales.Domain.Entities.Organizations;

namespace ZAP.Sales.Domain.Interfaces
{
    public interface IOrganizationRepository : IMongoRepository<OrganizationUnit>
    {
    }
}
