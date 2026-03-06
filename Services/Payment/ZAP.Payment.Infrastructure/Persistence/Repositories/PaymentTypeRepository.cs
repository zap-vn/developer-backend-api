using MongoDB.Driver;
using ZAP.BuildingBlocks.Interfaces;
using ZAP.BuildingBlocks.Repositories;
using ZAP.Payment.Domain.Entities;
using ZAP.Payment.Domain.Interfaces;

namespace ZAP.Payment.Infrastructure.Persistence.Repositories
{
    public class PaymentTypeRepository : BaseMongoRepository<PaymentType>, IPaymentTypeRepository
    {
        public PaymentTypeRepository(IMongoDatabase database, ICurrentUserService currentUserService) 
            : base(database, "PaymentTypes", currentUserService)
        {
        }
    }
}
