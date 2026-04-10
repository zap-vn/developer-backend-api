using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRM.Product.Domain.Entities;
using CRM.Product.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRM.Product.Infrastructure.Persistence.Repositories
{
    public class PostgresMenuRepository : IMenuRepository
    {
        private readonly PostgresDbContext _context;

        public PostgresMenuRepository(PostgresDbContext context)
        {
            _context = context;
        }

        public async Task<MenuHeader?> GetByIdAsync(Guid id)
        {
            return await _context.MenuHeaders
                .Include(x => x.sections)
                .Include(x => x.schedules)
                .FirstOrDefaultAsync(x => x.id == id);
        }

        public async Task<(IEnumerable<MenuHeader> Items, int Total)> GetPagedAsync(
            int page,
            int pageSize,
            Guid? tenantId = null,
            string? search = null,
            bool? isActive = null,
            string? menuType = null,
            int localeId = 2,
            string sortField = "name",
            bool sortDescending = false)
        {
            // Join with translations to get the status label
            var query = from m in _context.MenuHeaders
                        join st in _context.StatusItemTranslations.Where(t => t.locale_id == localeId)
                            on m.status_id equals st.status_item_id into stGroup
                        from st in stGroup.DefaultIfEmpty()
                        select new { m, StatusLabel = st != null ? st.name : "Unknown" };

            if (tenantId.HasValue)
                query = query.Where(x => x.m.tenant_id == tenantId);

            if (isActive.HasValue)
                query = query.Where(x => x.m.is_active == isActive.Value);

            if (!string.IsNullOrEmpty(menuType))
                query = query.Where(x => x.m.menu_type == menuType);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.m.name.Contains(search));
            }

            var total = await query.CountAsync();

            var ordered = sortDescending
                ? query.OrderByDescending(x => x.m.name)
                : query.OrderBy(x => x.m.name);

            if (sortField == "created_at" || sortField == "id")
            {
                ordered = sortDescending ? query.OrderByDescending(x => x.m.id) : query.OrderBy(x => x.m.id);
            }

            var results = await ordered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var menuIds = results.Select(x => x.m.id).ToList();

            // Populate total_items and sections_count
            var itemCounts = await (from s in _context.MenuSections
                                    join mi in _context.MenuItems on s.id equals mi.section_id
                                    where menuIds.Contains(s.menu_id)
                                    group mi by s.menu_id into g
                                    select new { MenuId = g.Key, Count = g.Count() })
                                   .ToDictionaryAsync(x => x.MenuId, x => x.Count);

            var sectionCounts = await _context.MenuSections
                                    .Where(s => menuIds.Contains(s.menu_id))
                                    .GroupBy(s => s.menu_id)
                                    .Select(g => new { MenuId = g.Key, Count = g.Count() })
                                    .ToDictionaryAsync(x => x.MenuId, x => x.Count);

            var items = new List<MenuHeader>();
            foreach (var res in results)
            {
                var menu = res.m;
                menu.StatusLabel = res.StatusLabel;
                menu.TotalItems = itemCounts.TryGetValue(menu.id, out var itmCount) ? itmCount : 0;
                menu.SectionsCount = sectionCounts.TryGetValue(menu.id, out var secCount) ? secCount : 0;
                items.Add(menu);
            }
            
            return (items, total);
        }

        public async Task CreateAsync(MenuHeader menu)
        {
            await _context.MenuHeaders.AddAsync(menu);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MenuHeader menu)
        {
            _context.MenuHeaders.Update(menu);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var menu = await _context.MenuHeaders.FindAsync(id);
            if (menu != null)
            {
                _context.MenuHeaders.Remove(menu);
                await _context.SaveChangesAsync();
            }
        }
    }
}
