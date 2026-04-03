using CRM.Product.Domain.Entities;
using CRM.Product.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

        public async Task<(IEnumerable<Category> Items, int Total)> GetPagedAsync(int page, int pageSize, Guid? tenantId = null, string? search = null)
        {
            var query = _context.Categories.AsNoTracking().Include(x => x.status).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                var lowerSearch = search.ToLower();
                query = query.Where(x => x.name.ToLower().Contains(lowerSearch));
            }

            var total = await query.CountAsync();
            var items = await query.OrderBy(x => x.name)
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
