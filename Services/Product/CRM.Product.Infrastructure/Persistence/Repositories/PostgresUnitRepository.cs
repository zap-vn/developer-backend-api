#nullable enable
using CRM.Product.Domain.Entities;
using CRM.Product.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CRM.Product.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Stub repository for UomItem. platform.uom_item table does not yet exist in the DB.
    /// Returns empty results to prevent startup failures.
    /// </summary>
    public class PostgresUnitRepository : IUnitRepository
    {
        public PostgresUnitRepository(PostgresDbContext context)
        {
            // context intentionally unused until platform.uom_item is provisioned
        }

        public Task<IEnumerable<UomItem>> GetAllAsync(Guid? tenantId = null)
            => Task.FromResult<IEnumerable<UomItem>>(Array.Empty<UomItem>());

        public Task<UomItem?> GetByIdAsync(Guid id)
            => Task.FromResult<UomItem?>(null);

        public Task<(IEnumerable<UomItem> Items, int Total)> GetPagedAsync(
            int page, int pageSize, Guid? tenantId = null, string? search = null)
            => Task.FromResult<(IEnumerable<UomItem>, int)>((Array.Empty<UomItem>(), 0));

        public Task CreateAsync(UomItem uomItem) => Task.CompletedTask;
        public Task UpdateAsync(UomItem uomItem) => Task.CompletedTask;
        public Task DeleteAsync(Guid id) => Task.CompletedTask;
    }
}
