using CRM.Product.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CRM.Product.Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<CRM.Product.Domain.Entities.Product>> GetAllAsync();
        Task<CRM.Product.Domain.Entities.Product?> GetByIdAsync(string id);
        Task CreateAsync(CRM.Product.Domain.Entities.Product product);
        Task UpdateAsync(CRM.Product.Domain.Entities.Product product);
        Task DeleteAsync(string id);
        Task<(IEnumerable<CRM.Product.Domain.Entities.Product> Items, int TotalCount)> GetPagedAsync(
            int page, 
            int pageSize, 
            Guid? tenantId = null,
            string? searchTerm = null,
            List<int>? statusIds = null);
    }
}

