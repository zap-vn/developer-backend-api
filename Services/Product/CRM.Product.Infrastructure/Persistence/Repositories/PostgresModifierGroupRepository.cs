using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRM.Product.Domain.Entities;
using CRM.Product.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRM.Product.Infrastructure.Persistence.Repositories
{
    public class PostgresModifierGroupRepository : IModifierGroupRepository
    {
        private readonly PostgresDbContext _context;

        public PostgresModifierGroupRepository(PostgresDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ModifierGroup>> GetAllAsync(Guid? tenantId = null)
        {
            var query = _context.ModifierGroups.AsQueryable();
            if (tenantId.HasValue) query = query.Where(x => x.tenant_id == tenantId);
            return await query.ToListAsync();
        }

        public async Task<ModifierGroup?> GetByIdAsync(Guid id)
        {
            return await _context.ModifierGroups.FindAsync(id);
        }

        public async Task<(IEnumerable<ModifierGroup> Items, int Total)> GetPagedAsync(
            int page,
            int pageSize,
            Guid? tenantId = null,
            string? search = null,
            int? statusId = null,
            string? displayType = null,
            string sortField = "name",
            bool sortDescending = false)
        {
            var query = _context.ModifierGroups.Include(x => x.status).AsQueryable();

            if (tenantId.HasValue)
                query = query.Where(x => x.tenant_id == tenantId);

            if (statusId.HasValue)
                query = query.Where(x => x.status_id == statusId);

            // display_type derived from max_selection: "single"=1, "multi">1
            if (!string.IsNullOrEmpty(displayType))
            {
                if (displayType == "single")
                    query = query.Where(x => x.max_selection == 1);
                else if (displayType == "multi")
                    query = query.Where(x => x.max_selection > 1);
            }

            // Search by ID (exact), name, or legacy_id (internal name)
            if (!string.IsNullOrEmpty(search))
            {
                Guid? searchGuid = Guid.TryParse(search, out var sg) ? sg : (Guid?)null;
                query = searchGuid.HasValue
                    ? query.Where(x => x.id == searchGuid)
                    : query.Where(x =>
                        x.name.Contains(search) ||
                        (x.legacy_id != null && x.legacy_id.Contains(search)));
            }

            var total = await query.CountAsync();

            IOrderedQueryable<ModifierGroup> ordered = sortField switch
            {
                "sort_order" => sortDescending
                    ? query.OrderByDescending(x => x.sort_order).ThenByDescending(x => x.name)
                    : query.OrderBy(x => x.sort_order).ThenBy(x => x.name),
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

        public async Task CreateAsync(ModifierGroup modifierGroup)
        {
            await _context.ModifierGroups.AddAsync(modifierGroup);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ModifierGroup modifierGroup)
        {
            _context.ModifierGroups.Update(modifierGroup);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var modifierGroup = await _context.ModifierGroups.FindAsync(id);
            if (modifierGroup != null)
            {
                _context.ModifierGroups.Remove(modifierGroup);
                await _context.SaveChangesAsync();
            }
        }
    }
}
