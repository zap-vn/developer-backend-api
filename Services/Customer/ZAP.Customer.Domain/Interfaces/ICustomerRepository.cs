using ZAP.BuildingBlocks.Interfaces;
using ZAP.Customer.Domain.Entities;

namespace ZAP.Customer.Domain.Interfaces
{
    public interface ICustomerRepository : IMongoRepository<CustomerEntity>
    {
    }
}
