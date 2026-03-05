using ZAP.BuildingBlocks.Interfaces;
using ZAP.Product.Domain.Entities;

namespace ZAP.Product.Domain.Interfaces
{
    public interface IProductRepository : IMongoRepository<ProductEntity>
    {
        Task<IEnumerable<ProductEntity>> GetByCategoryAsync(string category);
    }
}
