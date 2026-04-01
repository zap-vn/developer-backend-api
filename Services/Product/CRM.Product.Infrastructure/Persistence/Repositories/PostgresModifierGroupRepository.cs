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

        public async Task<(IEnumerable<ModifierGroup> Items, int Total)> GetPagedAsync(int page, int pageSize, Guid? tenantId = null, string? search = null)
        {
            var query = _context.ModifierGroups.AsQueryable();
            if (tenantId.HasValue) query = query.Where(x => x.tenant_id == tenantId);
            if (!string.IsNullOrEmpty(search)) query = query.Where(x => x.name.Contains(search));

            var total = await query.CountAsync();
            var items = await query.OrderBy(x => x.sort_order).ThenBy(x => x.name)
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
