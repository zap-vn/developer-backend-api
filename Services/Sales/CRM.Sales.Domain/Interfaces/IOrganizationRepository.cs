using CRM.BuildingBlocks.Interfaces;
using CRM.Sales.Domain.Entities.Organizations;

namespace CRM.Sales.Domain.Interfaces
{
    public interface IOrganizationRepository : IMongoRepository<OrganizationUnit>
    {
    }
}
