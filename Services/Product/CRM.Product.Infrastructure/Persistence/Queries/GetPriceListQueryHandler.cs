using MediatR;
using Microsoft.EntityFrameworkCore;
using CRM.BuildingBlocks.Models;
using CRM.Product.Application.Features.Prices.DTOs;
using CRM.Product.Application.Features.Prices.Queries;
using CRM.Product.Infrastructure.Persistence;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.Product.Infrastructure.Persistence.Queries
{
    public class GetPriceListQueryHandler : IRequestHandler<GetPriceListQuery, PagedResult<PriceListDto>>
    {
        private readonly PostgresDbContext _context;

        public GetPriceListQueryHandler(PostgresDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<PriceListDto>> Handle(GetPriceListQuery request, CancellationToken cancellationToken)
        {
            // The query follows the user's logic:
            // catalog.menu_item_hd (Base) JOIN catalog.product_variant (SKU/Barcode)
            // LEFT JOIN catalog.menu_price_override (Price overrides for specific location)
            
            var query = from mi in _context.MenuItems
                        join pv in _context.ProductVariants on mi.product_variant_id equals pv.id
                        join p in _context.Products on pv.product_id equals p.id
                        join cm in _context.ProductCategoryMappings on p.id equals cm.product_id into cmGroup
                        from cm in cmGroup.DefaultIfEmpty()
                        join cat in _context.Categories on cm.category_id equals cat.id into catGroup
                        from cat in catGroup.DefaultIfEmpty()
                        join po in _context.MenuPriceOverrides.Where(o => o.location_id == request.LocationId) 
                             on pv.id equals po.product_variant_id into poGroup
                        from po in poGroup.DefaultIfEmpty()
                        where mi.tenant_id == request.TenantId
                        select new { mi, pv, cat, po };

            if (request.CategoryId.HasValue)
            {
                query = query.Where(x => x.cat.id == request.CategoryId.Value);
            }

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(x => x.mi.name.Contains(request.SearchTerm) || x.pv.sku_code.Contains(request.SearchTerm));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var results = await query
                .OrderBy(x => x.mi.name)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new PriceListDto
                {
                    id = x.mi.id,
                    name = x.mi.name,
                    sku_code = x.pv.sku_code,
                    barcode = x.pv.barcode,
                    base_price = x.pv.base_price.GetValueOrDefault(),
                    price_override = x.po != null ? x.po.price_override : null,
                    category_name = x.cat != null ? x.cat.name : "Uncategorized"
                })
                .ToListAsync(cancellationToken);

            return new PagedResult<PriceListDto>(results, totalCount, request.Page, request.PageSize);
        }
    }
}
