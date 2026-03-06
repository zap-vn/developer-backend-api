using MongoDB.Driver;
using ZAP.BuildingBlocks.Interfaces;
using ZAP.BuildingBlocks.Repositories;
using ZAP.Sales.Domain.Entities.Payments;
using ZAP.Sales.Domain.Interfaces;

namespace ZAP.Sales.Infrastructure.Persistence.Repositories
{
    public class PaymentMethodRepository : BaseMongoRepository<PaymentMethod>, IPaymentMethodRepository
    {
        public PaymentMethodRepository(IMongoDatabase database, ICurrentUserService currentUserService) 
            : base(database, "PaymentMethods", currentUserService)
        {
        }
    }
}
