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
        Task<(IEnumerable<CRM.Product.Domain.Entities.ProductVariant> Items, int TotalCount)> GetPagedAsync(
            int page,
            int pageSize,
            Guid? tenantId = null,
            string? searchTerm = null,
            int? statusId = null,
            Guid? categoryId = null,
            Guid? locationId = null,
            int localeId = 2,
            int? productTypeId = null,
            string sortField = "created_at",
            bool sortDescending = true);

        Task<(IEnumerable<CRM.Product.Domain.Entities.Product> Items, int TotalCount)> GetPagedProductsAsync(
            int page,
            int pageSize,
            Guid? tenantId = null,
            string? searchTerm = null,
            int? statusId = null,
            Guid? categoryId = null,
            Guid? locationId = null,
            int localeId = 2,
            int? productTypeId = null,
            string sortField = "created_at",
            bool sortDescending = true);
    }
}

