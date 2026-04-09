using CRM.BuildingBlocks.Interfaces;
using CRM.BuildingBlocks.Models;
using CRM.Product.Application.Features.Products.DTOs;
using CRM.Product.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.Product.Application.Features.Products.Queries
{
    public class GetUnitsQueryHandler : IRequestHandler<GetUnitsQuery, PagedResult<UnitDto>>
    {
        private readonly IUnitRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public GetUnitsQueryHandler(IUnitRepository repository, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<PagedResult<UnitDto>> Handle(GetUnitsQuery request, CancellationToken cancellationToken)
        {
            var tenantIdString = _currentUserService.UserGuid;
            Guid? tenantId = null;
            if (Guid.TryParse(tenantIdString, out var guid)) tenantId = guid;

            var (items, total) = await _repository.GetPagedAsync(
                request.Request.PageIndex,
                request.Request.PageSize,
                tenantId,
                request.Request.Search,
                request.Request.Filters?.StatusId,
                request.Request.Filters?.Precision,
                request.Request.Sort?.Field ?? "name",
                request.Request.Sort?.Descending ?? false);

            var localeId = _currentUserService.LocaleId;
            var dtos = items.Select(x => new UnitDto
            {
                id = x.id,
                tenant_id = x.tenant_id,
                code = x.code,
                name = x.translations?.FirstOrDefault(t => t.locale_id == localeId)?.name ?? x.name,
                abbreviation = x.abbreviation,
                precision = x.precision,
                status_id = x.status_id,
                status_code = x.status?.code,
                status_name = x.status?.translations?.FirstOrDefault(t => t.locale_id == localeId)?.name ?? 
                              x.status?.translations?.FirstOrDefault(t => t.locale_id == 1)?.name,
                is_active = x.status_id != 0
            });

            return new PagedResult<UnitDto>(dtos.ToList(), total, request.Request.PageIndex, request.Request.PageSize);
        }
    }
}
