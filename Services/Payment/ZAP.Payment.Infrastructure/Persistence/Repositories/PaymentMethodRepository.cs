using ZAP.BuildingBlocks.Interfaces;
using ZAP.BuildingBlocks.Repositories;
using ZAP.Payment.Domain.Entities;
using ZAP.Payment.Domain.Interfaces;

namespace ZAP.Payment.Infrastructure.Persistence.Repositories
{
    public class PaymentMethodRepository : BaseMongoRepository<PaymentMethod>, IPaymentMethodRepository
    {
        public PaymentMethodRepository(MongoDbContext context, ICurrentUserService currentUserService) 
            : base(context.Database, "PaymentMethods", currentUserService)
        {
        }
    }
}
