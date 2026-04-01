using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRM.Product.Domain.Entities;
using CRM.Product.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRM.Product.Infrastructure.Persistence.Repositories
{
    public class PostgresBrandRepository : IBrandRepository
    {
        private readonly PostgresDbContext _context;

        public PostgresBrandRepository(PostgresDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Brand>> GetAllAsync(Guid? tenantId = null)
        {
            var query = _context.Brands.AsQueryable();
            if (tenantId.HasValue) query = query.Where(x => x.tenant_id == tenantId);
            return await query.ToListAsync();
        }

        public async Task<Brand?> GetByIdAsync(Guid id)
        {
            return await _context.Brands.FindAsync(id);
        }

        public async Task<(IEnumerable<Brand> Items, int Total)> GetPagedAsync(int page, int pageSize, Guid? tenantId = null, string? search = null)
        {
            var query = _context.Brands.AsQueryable();
            if (tenantId.HasValue) query = query.Where(x => x.tenant_id == tenantId);
            if (!string.IsNullOrEmpty(search)) query = query.Where(x => x.name.Contains(search));

            var total = await query.CountAsync();
            var items = await query.OrderBy(x => x.name)
                                   .Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();
            return (items, total);
        }

        public async Task CreateAsync(Brand brand)
        {
            await _context.Brands.AddAsync(brand);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Brand brand)
        {
            _context.Brands.Update(brand);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand != null)
            {
                _context.Brands.Remove(brand);
                await _context.SaveChangesAsync();
            }
        }
    }
}
