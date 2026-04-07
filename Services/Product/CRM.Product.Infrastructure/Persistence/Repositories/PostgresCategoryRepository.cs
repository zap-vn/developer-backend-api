using CRM.Product.Domain.Entities;
using CRM.Product.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.Product.Infrastructure.Persistence.Repositories
{
    public class PostgresCategoryRepository : ICategoryRepository
    {
        private readonly PostgresDbContext _context;

        public PostgresCategoryRepository(PostgresDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(Guid id)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(x => x.id == id);
        }

        public async Task<(IEnumerable<Category> Items, int Total)> GetPagedAsync(
            int page,
            int pageSize,
            Guid? tenantId = null,
            string? search = null,
            int? statusId = null,
            string sortField = "name",
            bool sortDescending = false)
        {
            var query = _context.Categories.AsNoTracking().Include(x => x.status).AsQueryable();

            if (tenantId.HasValue)
                query = query.Where(x => x.tenant_id == tenantId);

            // Filter by status
            if (statusId.HasValue)
                query = query.Where(x => x.status_id == statusId);

            // Search by ID (exact GUID match) or name (contains)
            if (!string.IsNullOrEmpty(search))
            {
                Guid? searchGuid = Guid.TryParse(search, out var sg) ? sg : (Guid?)null;
                query = searchGuid.HasValue
                    ? query.Where(x => x.id == searchGuid)
                    : query.Where(x => x.name.ToLower().Contains(search.ToLower()));
            }

            var total = await query.CountAsync();

            // Dynamic sort: "name" (default) or "sort_order"
            IOrderedQueryable<Category> ordered = sortField switch
            {
                "sort_order" => sortDescending
                    ? query.OrderByDescending(x => x.sort_order ?? int.MaxValue).ThenByDescending(x => x.name)
                    : query.OrderBy(x => x.sort_order ?? int.MaxValue).ThenBy(x => x.name),
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

        public async Task CreateAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
    }
}
