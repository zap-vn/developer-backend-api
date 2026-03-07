using CRM.BuildingBlocks.Interfaces;
using CRM.BuildingBlocks.Repositories;
using CRM.Customer.Domain.Entities;
using CRM.Customer.Domain.Interfaces;

namespace CRM.Customer.Infrastructure.Persistence.Repositories
{
    public class CustomerGroupRepository : BaseMongoRepository<CustomerGroup>, ICustomerGroupRepository
    {
        public CustomerGroupRepository(MongoDbContext context, ICurrentUserService currentUserService) 
            : base(context.Database, "CustomerGroups", currentUserService)
        {
        }
    }
}
