using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CRM.Product.Domain.Entities;

namespace CRM.Product.Domain.Interfaces
{
    public interface IMenuRepository
    {
        Task<MenuHeader?> GetByIdAsync(Guid id);
        Task<(IEnumerable<MenuHeader> Items, int Total)> GetPagedAsync(
            int page,
            int pageSize,
            Guid? tenantId = null,
            string? search = null,
            bool? isActive = null,
            string? menuType = null,
            int localeId = 2,
            string sortField = "name",
            bool sortDescending = false);
        Task CreateAsync(MenuHeader menu);
        Task UpdateAsync(MenuHeader menu);
        Task DeleteAsync(Guid id);
    }
}
