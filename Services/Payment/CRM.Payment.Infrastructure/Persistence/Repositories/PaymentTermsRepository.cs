using MongoDB.Driver;
using CRM.BuildingBlocks.Interfaces;
using CRM.BuildingBlocks.Repositories;
using CRM.Payment.Domain.Entities;
using CRM.Payment.Domain.Interfaces;

namespace CRM.Payment.Infrastructure.Persistence.Repositories
{
    public class PaymentTermsRepository : BaseMongoRepository<PaymentTerms>, IPaymentTermsRepository
    {
        public PaymentTermsRepository(IMongoDatabase database, ICurrentUserService currentUserService) 
            : base(database, "PaymentTerms", currentUserService)
        {
        }
    }
}
