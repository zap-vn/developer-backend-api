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
    public class GetModifierGroupsQueryHandler : IRequestHandler<GetModifierGroupsQuery, PagedResult<ModifierGroupDto>>
    {
        private readonly IModifierGroupRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public GetModifierGroupsQueryHandler(IModifierGroupRepository repository, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<PagedResult<ModifierGroupDto>> Handle(GetModifierGroupsQuery request, CancellationToken cancellationToken)
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
                request.Request.Filters?.DisplayType,
                request.Request.Sort?.Field ?? "name",
                request.Request.Sort?.Descending ?? false);

            var dtos = items.Select(x => new ModifierGroupDto
            {
                id = x.id,
                serial_id = x.serial_id,
                tenant_id = x.tenant_id,
                legacy_id = x.legacy_id,
                name = x.name,
                min_selection = x.min_selection,
                max_selection = x.max_selection,
                is_required = x.is_required,
                sort_order = x.sort_order,
                status_id = x.status_id,
                status_code = x.status?.code
            });

            return new PagedResult<ModifierGroupDto>(dtos.ToList(), total, request.Request.PageIndex, request.Request.PageSize);
        }
    }
}
