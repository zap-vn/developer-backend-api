using MongoDB.Driver;
using CRM.BuildingBlocks.Interfaces;
using CRM.BuildingBlocks.Repositories;
using CRM.Sales.Domain.Entities.Payments;
using CRM.Sales.Domain.Interfaces;

namespace CRM.Sales.Infrastructure.Persistence.Repositories
{
    public class PaymentMethodRepository : BaseMongoRepository<PaymentMethod>, IPaymentMethodRepository
    {
        public PaymentMethodRepository(IMongoDatabase database, ICurrentUserService currentUserService) 
            : base(database, "PaymentMethods", currentUserService)
        {
        }
    }
}
