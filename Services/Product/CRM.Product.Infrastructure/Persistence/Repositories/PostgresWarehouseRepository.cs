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

        public async Task<IEnumerable<Warehouse>> GetPagedAsync(LocationListFilter filter)
        {
            var query = BuildQuery(filter)
                .Include(x => x.status)
                .Include(x => x.location_type);

            query = ApplySort(query, filter);

            return await query
                .Skip((filter.PageIndex - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync(LocationListFilter filter)
        {
            return await BuildQuery(filter).CountAsync();
        }

        private IQueryable<Warehouse> BuildQuery(LocationListFilter filter)
        {
            var query = _context.Warehouses.AsQueryable();

            if (filter.TenantId.HasValue)
                query = query.Where(x => x.tenant_id == filter.TenantId);

            // Search: name
            if (!string.IsNullOrWhiteSpace(filter.SearchName))
                query = query.Where(x => x.name.Contains(filter.SearchName));

            // Search: node_code or id
            if (!string.IsNullOrWhiteSpace(filter.SearchLocationId))
            {
                var term = filter.SearchLocationId;
                if (Guid.TryParse(term, out var idGuid))
                    query = query.Where(x => x.id == idGuid || x.node_code!.Contains(term));
                else
                    query = query.Where(x => x.node_code!.Contains(term));
            }

            // Search: address
            if (!string.IsNullOrWhiteSpace(filter.SearchAddress))
                query = query.Where(x => x.address_line_1!.Contains(filter.SearchAddress));

            // Filter: status
            if (filter.StatusId.HasValue)
                query = query.Where(x => x.status_id == filter.StatusId);

            // Filter: province
            if (filter.ProvinceId.HasValue)
                query = query.Where(x => x.province_id == filter.ProvinceId);

            // Filter: location type (tier level)
            if (filter.LocationTypeId.HasValue)
                query = query.Where(x => x.location_type_id == filter.LocationTypeId);

            return query;
        }

        private IQueryable<Warehouse> ApplySort(IQueryable<Warehouse> query, LocationListFilter filter)
        {
            return filter.SortField?.ToLower() switch
            {
                "node_code" => filter.SortDescending
                    ? query.OrderByDescending(x => x.node_code)
                    : query.OrderBy(x => x.node_code),
                "status" => filter.SortDescending
                    ? query.OrderByDescending(x => x.status_id)
                    : query.OrderBy(x => x.status_id),
                _ => filter.SortDescending
                    ? query.OrderByDescending(x => x.name)
                    : query.OrderBy(x => x.name)
            };
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
