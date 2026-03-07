using MongoDB.Driver;
using CRM.BuildingBlocks.Interfaces;
using CRM.BuildingBlocks.Repositories;
using CRM.Customer.Domain.Entities;
using CRM.Customer.Domain.Interfaces;

namespace CRM.Customer.Infrastructure.Persistence.Repositories
{
    public class CustomerRepository : BaseMongoRepository<CustomerEntity>, ICustomerRepository
    {
        public CustomerRepository(IMongoDatabase database, ICurrentUserService currentUserService) 
            : base(database, "Customers", currentUserService)
        {
        }
    }
}
