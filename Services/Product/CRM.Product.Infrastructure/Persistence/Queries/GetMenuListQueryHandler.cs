using MediatR;
using Microsoft.EntityFrameworkCore;
using CRM.BuildingBlocks.Models;
using CRM.Product.Application.Features.Menus.DTOs;
using CRM.Product.Application.Features.Menus.Queries;
using CRM.Product.Infrastructure.Persistence;
using CRM.Product.Domain.Interfaces;
using CRM.BuildingBlocks.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.Product.Infrastructure.Persistence.Queries
{
    public class GetMenuListQueryHandler : IRequestHandler<GetMenuListQuery, PagedResult<MenuListResultDto>>
    {
        private readonly IMenuRepository _repository;
        private readonly ILocalizationService _localizationService;

        public GetMenuListQueryHandler(IMenuRepository repository, ILocalizationService localizationService)
        {
            _repository = repository;
            _localizationService = localizationService;
        }

        public async Task<PagedResult<MenuListResultDto>> Handle(GetMenuListQuery request, CancellationToken cancellationToken)
        {
            var req = request.Request;
            var localeId = _localizationService.GetCurrentLocaleId();
            
            var (items, totalCount) = await _repository.GetPagedAsync(
                req.PageIndex,
                req.PageSize,
                request.TenantId,
                req.Search,
                req.Filters?.IsActive,
                req.Filters?.MenuType,
                localeId,
                req.Sort?.Field ?? "name",
                req.Sort?.Descending ?? false
            );

            var results = items.Select(m => new MenuListResultDto
            {
                id = m.id,
                name = m.name,
                menu_type = m.menu_type,
                app_id = m.app_id?.ToString(),
                status_id = m.status_id,
                status_code = m.status?.code,
                status_name = m.status != null
                    ? (m.status.translations?.FirstOrDefault(t => t.locale_id == localeId)?.name ?? 
                       $"{m.status.translations?.FirstOrDefault(t => t.locale_id == 2)?.name} ({m.status.translations?.FirstOrDefault(t => t.locale_id == 1)?.name})")
                    : null,
                timezone_id = m.timezone_id,
                is_active = m.is_active,
                sections_count = m.SectionsCount,
                total_items = m.TotalItems
            }).ToList();

            return new PagedResult<MenuListResultDto>(results, totalCount, req.PageIndex, req.PageSize);
        }
    }
}
