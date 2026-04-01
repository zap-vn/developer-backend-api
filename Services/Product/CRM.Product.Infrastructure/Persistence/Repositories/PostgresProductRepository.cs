#nullable enable
using Microsoft.EntityFrameworkCore;
using CRM.Product.Domain.Entities;
using CRM.Product.Domain.Interfaces;
using CRM.Product.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace CRM.Product.Infrastructure.Persistence.Repositories
{
    public class PostgresProductRepository : IProductRepository
    {
        private readonly PostgresDbContext _context;

        public PostgresProductRepository(PostgresDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CRM.Product.Domain.Entities.Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.variants)
                .ToListAsync();
        }

        public async Task<CRM.Product.Domain.Entities.Product?> GetByIdAsync(string id)
        {
            if (!Guid.TryParse(id, out var guid)) return null;
            return await _context.Products
                .Include(p => p.variants)
                .FirstOrDefaultAsync(p => p.id == guid);
        }

        public async Task CreateAsync(CRM.Product.Domain.Entities.Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CRM.Product.Domain.Entities.Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            if (!Guid.TryParse(id, out var guid)) return;
            var product = await _context.Products.FindAsync(guid);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<(IEnumerable<CRM.Product.Domain.Entities.Product> Items, int TotalCount)> GetPagedAsync(
            int page, 
            int pageSize, 
            Guid? tenantId = null,
            string? searchTerm = null,
            List<int>? statusIds = null)
        {
            var query = _context.Products
                .Include(p => p.variants)
                .AsQueryable();

            if (tenantId.HasValue)
                query = query.Where(x => x.tenant_id == tenantId);

            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Where(x => x.name.Contains(searchTerm) || (x.legacy_id != null && x.legacy_id.Contains(searchTerm)));

            if (statusIds != null && statusIds.Any())
                query = query.Where(x => x.status_id.HasValue && statusIds.Contains(x.status_id.Value));

            int total = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            return (items, total);
        }
    }
}
