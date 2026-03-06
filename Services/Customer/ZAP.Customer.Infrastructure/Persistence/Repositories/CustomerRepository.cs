using MongoDB.Driver;
using ZAP.BuildingBlocks.Interfaces;
using ZAP.BuildingBlocks.Repositories;
using ZAP.Customer.Domain.Entities;
using ZAP.Customer.Domain.Interfaces;

namespace ZAP.Customer.Infrastructure.Persistence.Repositories
{
    public class CustomerRepository : BaseMongoRepository<CustomerEntity>, ICustomerRepository
    {
        public CustomerRepository(IMongoDatabase database, ICurrentUserService currentUserService) 
            : base(database, "Customers", currentUserService)
        {
        }
    }
}
