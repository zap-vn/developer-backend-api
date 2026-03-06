using ZAP.BuildingBlocks.Interfaces;
using ZAP.Sales.Domain.Entities.Payments;

namespace ZAP.Sales.Domain.Interfaces
{
    public interface IPaymentMethodRepository : IMongoRepository<PaymentMethod>
    {
    }
}
