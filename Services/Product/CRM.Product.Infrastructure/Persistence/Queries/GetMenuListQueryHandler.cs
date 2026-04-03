using MediatR;
using Microsoft.EntityFrameworkCore;
using CRM.BuildingBlocks.Models;
using CRM.Product.Application.Features.Menus.DTOs;
using CRM.Product.Application.Features.Menus.Queries;
using CRM.Product.Infrastructure.Persistence;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.Product.Infrastructure.Persistence.Queries
{
    public class GetMenuListQueryHandler : IRequestHandler<GetMenuListQuery, PagedResult<MenuListResultDto>>
    {
        private readonly PostgresDbContext _context;

        public GetMenuListQueryHandler(PostgresDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<MenuListResultDto>> Handle(GetMenuListQuery request, CancellationToken cancellationToken)
        {
            var query = _context.MenuHeaders
                .Include(m => m.sections)
                .AsNoTracking();

            if (request.TenantId.HasValue)
            {
                query = query.Where(m => m.tenant_id == request.TenantId.Value);
            }

            if (!string.IsNullOrEmpty(request.Name))
            {
                query = query.Where(m => m.name.Contains(request.Name));
            }

            if (!string.IsNullOrEmpty(request.MenuType))
            {
                query = query.Where(m => m.menu_type == request.MenuType);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(m => m.is_active == request.IsActive.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var results = await query
                .OrderBy(m => m.name)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(m => new MenuListResultDto
                {
                    id = m.id,
                    name = m.name,
                    menu_type = m.menu_type,
                    timezone_id = m.timezone_id,
                    is_active = m.is_active,
                    sections_count = m.sections.Count
                })
                .ToListAsync(cancellationToken);

            return new PagedResult<MenuListResultDto>(results, totalCount, request.Page, request.PageSize);
        }
    }
}
