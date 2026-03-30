using MongoDB.Driver;
using CRM.BuildingBlocks.Interfaces;
using CRM.BuildingBlocks.Repositories;
using CRM.Payment.Domain.Entities;
using CRM.Payment.Domain.Interfaces;

namespace CRM.Payment.Infrastructure.Persistence.Repositories
{
    public class PaymentTypeRepository : BaseMongoRepository<PaymentType>, IPaymentTypeRepository
    {
        public PaymentTypeRepository(IMongoDatabase database, ICurrentUserService currentUserService) 
            : base(database, "payment.PaymentTypes", currentUserService)
        {
        }
    }
}
