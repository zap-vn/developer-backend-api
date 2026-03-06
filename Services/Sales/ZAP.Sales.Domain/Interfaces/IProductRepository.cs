using System.Collections.Generic;
using System.Threading.Tasks;
using ZAP.BuildingBlocks.Interfaces;
using ZAP.Sales.Domain.Entities.Products;

namespace ZAP.Sales.Domain.Interfaces
{
    public interface IProductRepository : IMongoRepository<ProductEntity>
    {
        Task<IEnumerable<ProductEntity>> GetByCategoryAsync(string category);
    }
}
