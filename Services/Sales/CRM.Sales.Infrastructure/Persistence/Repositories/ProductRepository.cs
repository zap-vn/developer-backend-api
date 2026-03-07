using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using CRM.BuildingBlocks.Interfaces;
using CRM.BuildingBlocks.Repositories;
using CRM.Sales.Domain.Entities.Products;
using CRM.Sales.Domain.Interfaces;

namespace CRM.Sales.Infrastructure.Persistence.Repositories
{
    public class ProductRepository : BaseMongoRepository<ProductEntity>, IProductRepository
    {
        public ProductRepository(IMongoDatabase database, ICurrentUserService currentUserService) 
            : base(database, "Products", currentUserService)
        {
        }

        public async Task<IEnumerable<ProductEntity>> GetByCategoryAsync(string category)
        {
            return await FindAsync(p => p.Category == category);
        }
    }
}
