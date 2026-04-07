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
    public class PostgresLocationRepository : ILocationRepository
    {
        private readonly PostgresDbContext _context;

        public PostgresLocationRepository(PostgresDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Location>> GetPagedAsync(LocationListFilter filter)
        {
            IQueryable<Location> query = BuildQuery(filter)
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

        private IQueryable<Location> BuildQuery(LocationListFilter filter)
        {
            var query = _context.Locations.AsQueryable();

            if (filter.TenantId.HasValue)
                query = query.Where(x => x.tenant_id == filter.TenantId);

            // Search
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var term = filter.Search;
                bool isGuid = Guid.TryParse(term, out var idGuid);
                
                query = query.Where(x => 
                    EF.Functions.ILike(x.name, $"%{term}%") || 
                    EF.Functions.ILike(EF.Functions.Unaccent(x.name), $"%{term}%") ||
                    (x.location_code != null && EF.Functions.ILike(x.location_code, $"%{term}%")) ||
                    (x.serial_number != null && EF.Functions.ILike(x.serial_number, $"%{term}%")) ||
                    (isGuid && x.id == idGuid) ||
                    (x.address_line_1 != null && EF.Functions.ILike(x.address_line_1, $"%{term}%")) ||
                    (x.address_line_1 != null && EF.Functions.ILike(EF.Functions.Unaccent(x.address_line_1), $"%{term}%"))
                );
            }

            // Filter: status
            if (filter.StatusId.HasValue)
                query = query.Where(x => x.status_id == filter.StatusId);

            // Filter: province
            if (filter.ProvinceId.HasValue)
                query = query.Where(x => x.province_id == filter.ProvinceId);

            // Filter: location type (list string)
            if (filter.LocationTypeIds != null && filter.LocationTypeIds.Count > 0)
            {
                // Parse string IDs to int for comparison
                var ids = filter.LocationTypeIds
                    .Select(s => int.TryParse(s, out var n) ? (int?)n : null)
                    .Where(n => n.HasValue)
                    .Select(n => n!.Value)
                    .ToList();
                if (ids.Count > 0)
                    query = query.Where(x => x.location_type_id != null && ids.Contains(x.location_type_id.Value));
            }

            return query;
        }

        private IQueryable<Location> ApplySort(IQueryable<Location> query, LocationListFilter filter)
        {
            return filter.SortField?.ToLower() switch
            {
                "status" => filter.SortDescending
                    ? query.OrderByDescending(x => x.status_id)
                    : query.OrderBy(x => x.status_id),
                _ => filter.SortDescending
                    ? query.OrderByDescending(x => x.name)
                    : query.OrderBy(x => x.name)
            };
        }

        public async Task<Location?> GetByIdAsync(Guid id)
        {
            return await _context.Locations
                .Include(x => x.location_type)
                .Include(x => x.status)
                .FirstOrDefaultAsync(w => w.id == id);
        }

        public async Task CreateAsync(Location location)
        {
            await _context.Locations.AddAsync(location);
            await _context.SaveChangesAsync();
        }

        public async Task CreateStoreAsync(Store store)
        {
            await _context.Stores.AddAsync(store);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Location location)
        {
            _context.Locations.Update(location);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var location = await _context.Locations.FindAsync(id);
            if (location != null)
            {
                _context.Locations.Remove(location);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ProvinceItem>> GetProvincesAsync(int localeId)
        {
            return await _context.ProvinceItems
                .Include(p => p.translations.Where(t => t.locale_id == localeId))
                .Where(p => p.is_active)
                .ToListAsync();
        }
    }
}
