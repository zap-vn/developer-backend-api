using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CRM.Product.Domain.Entities;

namespace CRM.Product.Domain.Interfaces
{
    public interface IBrandRepository
    {
        Task<IEnumerable<Brand>> GetAllAsync(Guid? tenantId = null);
        Task<Brand?> GetByIdAsync(Guid id);
        Task<(IEnumerable<Brand> Items, int Total)> GetPagedAsync(
            int page,
            int pageSize,
            Guid? tenantId = null,
            string? search = null,
            int? statusId = null,
            string sortField = "name",
            bool sortDescending = false);
        Task CreateAsync(Brand brand);
        Task UpdateAsync(Brand brand);
        Task DeleteAsync(Guid id);
    }
}
