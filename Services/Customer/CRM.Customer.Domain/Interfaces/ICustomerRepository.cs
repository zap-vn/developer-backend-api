using CRM.BuildingBlocks.Interfaces;
using CRM.Customer.Domain.Entities;

namespace CRM.Customer.Domain.Interfaces
{
    public interface ICustomerRepository : IMongoRepository<CustomerEntity>
    {
    }
}
