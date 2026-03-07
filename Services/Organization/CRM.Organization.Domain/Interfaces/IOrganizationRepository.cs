using CRM.BuildingBlocks.Interfaces;
using CRM.Organization.Domain.Entities;

namespace CRM.Organization.Domain.Interfaces
{
    public interface IOrganizationRepository : IMongoRepository<OrganizationUnit>
    {
    }
}
