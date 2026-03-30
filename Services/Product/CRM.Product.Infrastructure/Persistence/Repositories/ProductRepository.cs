using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using CRM.BuildingBlocks.Interfaces;
using CRM.BuildingBlocks.Repositories;
using CRM.Product.Domain.Entities;
using CRM.Product.Domain.Interfaces;

namespace CRM.Product.Infrastructure.Persistence.Repositories
{
    public class ProductRepository : BaseMongoRepository<ProductEntity>, IProductRepository
    {
        public ProductRepository(IMongoDatabase database, ICurrentUserService currentUserService) 
            : base(database, "catalog.Products", currentUserService)
        {
        }

        public async Task<IEnumerable<ProductEntity>> GetByCategoryAsync(string category)
        {
            return await FindAsync(p => p.Category == category);
        }
    }
}
