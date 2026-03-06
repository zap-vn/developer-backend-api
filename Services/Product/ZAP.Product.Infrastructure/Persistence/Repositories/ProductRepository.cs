using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZAP.BuildingBlocks.Interfaces;
using ZAP.BuildingBlocks.Repositories;
using ZAP.Product.Domain.Entities;
using ZAP.Product.Domain.Interfaces;

namespace ZAP.Product.Infrastructure.Persistence.Repositories
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
