using CRM.BuildingBlocks.Interfaces;
using CRM.Product.Domain.Entities;

namespace CRM.Product.Domain.Interfaces
{
    public interface IProductRepository : IMongoRepository<ProductEntity>
    {
    }
}
