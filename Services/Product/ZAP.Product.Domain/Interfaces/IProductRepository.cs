using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZAP.Product.Domain.Entities;

namespace ZAP.Product.Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<ProductEntity> GetByIdAsync(Guid id);
        Task<IEnumerable<ProductEntity>> GetAllAsync();
        Task<ProductEntity> CreateAsync(ProductEntity product);
        Task<bool> UpdateAsync(ProductEntity product);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<ProductEntity>> GetByCategoryAsync(string category);
    }
}
