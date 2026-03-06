using ZAP.BuildingBlocks.Interfaces;
using ZAP.Payment.Domain.Entities;

namespace ZAP.Payment.Domain.Interfaces
{
    public interface IPaymentTypeRepository : IMongoRepository<PaymentType>
    {
    }
}
