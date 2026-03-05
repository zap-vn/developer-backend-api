using ZAP.BuildingBlocks.Interfaces;
using ZAP.BuildingBlocks.Repositories;
using ZAP.Customer.Domain.Entities;
using ZAP.Customer.Domain.Interfaces;

namespace ZAP.Customer.Infrastructure.Persistence.Repositories
{
    public class CustomerGroupRepository : BaseMongoRepository<CustomerGroup>, ICustomerGroupRepository
    {
        public CustomerGroupRepository(MongoDbContext context, ICurrentUserService currentUserService) 
            : base(context.Database, "CustomerGroups", currentUserService)
        {
        }
    }
}
