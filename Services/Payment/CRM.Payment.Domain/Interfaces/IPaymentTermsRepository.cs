using CRM.BuildingBlocks.Interfaces;
using CRM.Payment.Domain.Entities;

namespace CRM.Payment.Domain.Interfaces
{
    public interface IPaymentTermsRepository : IMongoRepository<PaymentTerms>
    {
    }
}
