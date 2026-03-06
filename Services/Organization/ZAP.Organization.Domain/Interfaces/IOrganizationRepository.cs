using ZAP.BuildingBlocks.Interfaces;
using ZAP.Organization.Domain.Entities;

namespace ZAP.Organization.Domain.Interfaces
{
    public interface IOrganizationRepository : IMongoRepository<OrganizationUnit>
    {
    }
}
