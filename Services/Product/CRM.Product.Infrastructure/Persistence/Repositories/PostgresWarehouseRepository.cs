using Microsoft.EntityFrameworkCore;
using CRM.Product.Domain.Entities;
using CRM.Product.Domain.Interfaces;
using CRM.Product.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.Product.Infrastructure.Persistence.Repositories
{
    public class PostgresWarehouseRepository : IWarehouseRepository
    {
        private readonly PostgresDbContext _context;

        public PostgresWarehouseRepository(PostgresDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Warehouse>> GetPagedAsync(
            int page, 
            int pageSize, 
            Guid? tenantId = null,
            string? searchTerm = null)
        {
            var query = _context.Warehouses
                .Include(x => x.status)
                .Include(x => x.location_type)
                .AsQueryable();


            if (tenantId.HasValue)
                query = query.Where(x => x.tenant_id == tenantId);

            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Where(x => x.name.Contains(searchTerm));

            return await query.Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync(Guid? tenantId = null, string? searchTerm = null)
        {
            var query = _context.Warehouses.AsQueryable();

            if (tenantId.HasValue)
                query = query.Where(x => x.tenant_id == tenantId);

            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Where(x => x.name.Contains(searchTerm));

            return await query.CountAsync();
        }

        public async Task<Warehouse?> GetByIdAsync(Guid id)
        {
            return await _context.Warehouses
                .Include(x => x.location_type)
                .Include(x => x.status)
                .FirstOrDefaultAsync(w => w.id == id);
        }

        public async Task CreateAsync(Warehouse warehouse)
        {
            await _context.Warehouses.AddAsync(warehouse);
            await _context.SaveChangesAsync();
        }

        public async Task CreateStoreAsync(Store store)
        {
            await _context.Stores.AddAsync(store);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Warehouse warehouse)
        {
            _context.Warehouses.Update(warehouse);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse != null)
            {
                _context.Warehouses.Remove(warehouse);
                await _context.SaveChangesAsync();
            }
        }
    }
}
