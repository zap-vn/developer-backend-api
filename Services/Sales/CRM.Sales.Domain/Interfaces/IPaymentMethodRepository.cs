using CRM.BuildingBlocks.Interfaces;
using CRM.Sales.Domain.Entities.Payments;

namespace CRM.Sales.Domain.Interfaces
{
    public interface IPaymentMethodRepository : IMongoRepository<PaymentMethod>
    {
    }
}
