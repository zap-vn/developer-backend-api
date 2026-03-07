using System.Collections.Generic;
using System.Threading.Tasks;
using CRM.BuildingBlocks.Interfaces;
using CRM.Sales.Domain.Entities.Products;

namespace CRM.Sales.Domain.Interfaces
{
    public interface IProductRepository : IMongoRepository<ProductEntity>
    {
        Task<IEnumerable<ProductEntity>> GetByCategoryAsync(string category);
    }
}
