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
                .Include(p => p.variants).ThenInclude(v => v.media)
                .Include(p => p.variants).ThenInclude(v => v.location_pricing)
                .Include(p => p.category_mappings).ThenInclude(cm => cm.category)
                .Include(p => p.status).ThenInclude(s => s != null ? s.translations : null)
                .ToListAsync();
        }

        public async Task<CRM.Product.Domain.Entities.Product?> GetByIdAsync(string id)
        {
            if (!Guid.TryParse(id, out var guid)) return null;
            return await _context.Products
                .Include(p => p.variants).ThenInclude(v => v.media)
                .Include(p => p.variants).ThenInclude(v => v.location_pricing)
                .Include(p => p.category_mappings).ThenInclude(cm => cm.category)
                .Include(p => p.status).ThenInclude(s => s != null ? s.translations : null)
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

        public async Task<(IEnumerable<CRM.Product.Domain.Entities.ProductVariant> Items, int TotalCount)> GetPagedAsync(
            int page,
            int pageSize,
            Guid? tenantId = null,
            string? searchTerm = null,
            int? statusId = null,
            Guid? categoryId = null,
            Guid? locationId = null,
            int localeId = 2,
            int? productTypeId = null,
            string sortField = "created_at",
            bool sortDescending = true)
        {
            // Step 1: build filter query (no Includes — avoids EF Core materializer column-order issues)
            var query = _context.ProductVariants.AsQueryable();

            // Filter by tenant if provided
            if (tenantId.HasValue)
                query = query.Where(v => v.product != null && v.product.tenant_id == tenantId);

            // Filter by status: Exclude 0 (Draft/Deleted) by default, or use provided statusId
            if (statusId.HasValue)
            {
                query = query.Where(v => v.product != null && v.product.status_id == statusId);
            }
            else
            {
                query = query.Where(v => v.product != null && v.product.status_id != 0);
            }

            if (categoryId.HasValue)
                query = query.Where(v => _context.ProductCategoryMappings
                    .Any(cm => cm.product_id == v.product_id && cm.category_id == categoryId));

            if (locationId.HasValue)
                query = query.Where(v => v.inventory_items.Any(ii => ii.location_id == locationId));

            if (productTypeId.HasValue)
                query = query.Where(v => v.product != null && v.product.product_type_id == productTypeId);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                // If the term is a valid GUID, also match by variant id or product id
                Guid? searchGuid = Guid.TryParse(searchTerm, out var sg) ? sg : (Guid?)null;
                query = query.Where(v =>
                    (searchGuid.HasValue && (v.id == searchGuid || v.product_id == searchGuid)) ||
                    (v.variant_name != null && v.variant_name.Contains(searchTerm)) ||
                    (v.sku_code != null && v.sku_code.Contains(searchTerm)) ||
                    (v.barcode != null && v.barcode.Contains(searchTerm)) ||
                    (v.product != null && v.product.name.Contains(searchTerm)));
            }

            int total = await query.CountAsync();

            // Step 2: get paginated IDs only with dynamic sort
            IOrderedQueryable<CRM.Product.Domain.Entities.ProductVariant> orderedQuery = sortField switch
            {
                "name" => sortDescending
                    ? query.OrderByDescending(v => v.variant_name ?? (v.product != null ? v.product.name : "")).ThenByDescending(v => v.id)
                    : query.OrderBy(v => v.variant_name ?? (v.product != null ? v.product.name : "")).ThenBy(v => v.id),
                "price" => sortDescending
                    ? query.OrderByDescending(v => v.sale_price ?? v.base_price ?? 0).ThenByDescending(v => v.id)
                    : query.OrderBy(v => v.sale_price ?? v.base_price ?? 0).ThenBy(v => v.id),
                "stock" => sortDescending
                    ? query.OrderByDescending(v => v.inventory_items.Sum(i => (decimal?)i.qty_on_hand) ?? 0).ThenByDescending(v => v.id)
                    : query.OrderBy(v => v.inventory_items.Sum(i => (decimal?)i.qty_on_hand) ?? 0).ThenBy(v => v.id),
                _ => sortDescending
                    ? query.OrderByDescending(v => v.product != null ? v.product.created_at : DateTime.MinValue).ThenByDescending(v => v.id)
                    : query.OrderBy(v => v.product != null ? v.product.created_at : DateTime.MinValue).ThenBy(v => v.id),
            };

            var pagedIds = await orderedQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(v => v.id)
                .ToListAsync();

            if (!pagedIds.Any())
                return (Enumerable.Empty<CRM.Product.Domain.Entities.ProductVariant>(), total);

            // Step 3: load full data for those IDs — preserve page order from Step 2
            var rawItems = await _context.ProductVariants
                .Where(v => pagedIds.Contains(v.id))
                .Include(v => v.product!)
                    .ThenInclude(p => p.status!)
                        .ThenInclude(s => s.translations)
                .Include(v => v.product!)
                    .ThenInclude(p => p.product_type!)
                        .ThenInclude(pt => pt.translations)
                .Include(v => v.product!)
                    .ThenInclude(p => p.category_mappings)
                        .ThenInclude(cm => cm.category)
                .Include(v => v.media.Where(m => m.is_primary))
                .Include(v => v.location_pricing.Where(lp => lp.is_active && (!locationId.HasValue || lp.location_id == locationId)))
                .Include(v => v.inventory_items.Where(ii => !locationId.HasValue || ii.location_id == locationId))
                    .ThenInclude(i => i.Location)
                .Include(v => v.bom_headers.Where(b => b.is_active))
                .Include(v => v.uom)
                .ToListAsync();

            // Re-apply the page order (pagedIds is already sorted correctly by Step 2)
            var items = pagedIds
                .Select(id => rawItems.FirstOrDefault(v => v.id == id))
                .Where(v => v != null)
                .Select(v => v!)
                .ToList();

            return (items, total);
        }

        public async Task<(IEnumerable<CRM.Product.Domain.Entities.Product> Items, int TotalCount)> GetPagedProductsAsync(
            int page,
            int pageSize,
            Guid? tenantId = null,
            string? searchTerm = null,
            int? statusId = null,
            Guid? categoryId = null,
            Guid? locationId = null,
            int localeId = 2,
            int? productTypeId = null,
            string sortField = "created_at",
            bool sortDescending = true)
        {
            var query = _context.Products.AsQueryable();

            if (tenantId.HasValue)
                query = query.Where(p => p.tenant_id == tenantId);

            if (statusId.HasValue)
                query = query.Where(p => p.status_id == statusId);
            else
                query = query.Where(p => p.status_id != 0);

            if (categoryId.HasValue)
                query = query.Where(p => p.category_mappings.Any(cm => cm.category_id == categoryId));

            if (locationId.HasValue)
                query = query.Where(p => p.variants.Any(v => v.inventory_items.Any(ii => ii.location_id == locationId)));

            if (productTypeId.HasValue)
                query = query.Where(p => p.product_type_id == productTypeId);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => 
                    p.name.Contains(searchTerm) || 
                    p.variants.Any(v => (v.sku_code != null && v.sku_code.Contains(searchTerm)) || (v.variant_name != null && v.variant_name.Contains(searchTerm))));
            }

            int total = await query.CountAsync();

            IOrderedQueryable<CRM.Product.Domain.Entities.Product> orderedQuery = sortField switch
            {
                "name" => sortDescending ? query.OrderByDescending(p => p.name) : query.OrderBy(p => p.name),
                _ => sortDescending ? query.OrderByDescending(p => p.created_at) : query.OrderBy(p => p.created_at),
            };

            var pagedIds = await orderedQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => p.id)
                .ToListAsync();

            var rawItems = await _context.Products
                .Where(p => pagedIds.Contains(p.id))
                .Include(p => p.status).ThenInclude(s => s != null ? s.translations : null)
                .Include(p => p.product_type).ThenInclude(pt => pt.translations)
                .Include(p => p.category_mappings).ThenInclude(cm => cm.category)
                .Include(p => p.variants).ThenInclude(v => v.media)
                .Include(p => p.variants).ThenInclude(v => v.inventory_items).ThenInclude(i => i.Location)
                .Include(p => p.variants).ThenInclude(v => v.location_pricing)
                .Include(p => p.variants).ThenInclude(v => v.uom)
                .ToListAsync();

            var items = pagedIds
                .Select(id => rawItems.FirstOrDefault(p => p.id == id))
                .Where(p => p != null)
                .Select(p => p!)
                .ToList();

            return (items, total);
        }
    }
}
