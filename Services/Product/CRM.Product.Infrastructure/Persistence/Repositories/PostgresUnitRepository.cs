#nullable enable
using CRM.Product.Domain.Entities;
using CRM.Product.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.Product.Infrastructure.Persistence.Repositories
{
    public class PostgresUnitRepository : IUnitRepository
    {
        private readonly PostgresDbContext _context;

        public PostgresUnitRepository(PostgresDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UomItem>> GetAllAsync(Guid? tenantId = null)
        {
            var query = _context.UomItems.Include(x => x.status).AsQueryable();
            if (tenantId.HasValue) query = query.Where(x => x.tenant_id == tenantId);
            return await query.OrderBy(x => x.name).ToListAsync();
        }

        public async Task<UomItem?> GetByIdAsync(int id)
            => await _context.UomItems.Include(x => x.status).FirstOrDefaultAsync(x => x.id == id);

        public async Task<(IEnumerable<UomItem> Items, int Total)> GetPagedAsync(
            int page,
            int pageSize,
            Guid? tenantId = null,
            string? search = null,
            int? statusId = null,
            int? precision = null,
            string sortField = "name",
            bool sortDescending = false)
        {
            var query = _context.UomItems.Include(x => x.status).AsQueryable();

            if (tenantId.HasValue)
                query = query.Where(x => x.tenant_id == tenantId);

            if (statusId.HasValue)
                query = query.Where(x => x.status_id == statusId);

            if (precision.HasValue)
                query = query.Where(x => x.precision == precision);

            // Search across name and abbreviation (symbol)
            if (!string.IsNullOrEmpty(search))
                query = query.Where(x =>
                    x.name.Contains(search) ||
                    (x.abbreviation != null && x.abbreviation.Contains(search)));

            var total = await query.CountAsync();

            IOrderedQueryable<UomItem> ordered = sortField switch
            {
                "precision" => sortDescending
                    ? query.OrderByDescending(x => x.precision).ThenBy(x => x.name)
                    : query.OrderBy(x => x.precision).ThenBy(x => x.name),
                "status" => sortDescending
                    ? query.OrderByDescending(x => x.status_id).ThenBy(x => x.name)
                    : query.OrderBy(x => x.status_id).ThenBy(x => x.name),
                _ => sortDescending
                    ? query.OrderByDescending(x => x.name)
                    : query.OrderBy(x => x.name),
            };

            var items = await ordered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task CreateAsync(UomItem uomItem)
        {
            await _context.UomItems.AddAsync(uomItem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(UomItem uomItem)
        {
            _context.UomItems.Update(uomItem);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.UomItems.FindAsync(id);
            if (entity != null)
            {
                _context.UomItems.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
