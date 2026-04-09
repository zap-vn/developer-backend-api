using MediatR;
using CRM.Customer.Application.Features.Customers.DTOs;
using CRM.Customer.Domain.Interfaces;
using CRM.BuildingBlocks.Interfaces;
using CRM.BuildingBlocks.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.Customer.Application.Features.Customers.Queries
{
    public class GetCustomerListQueryHandler : IRequestHandler<GetCustomerListQuery, PagedResult<CustomerListDto>>
    {
        private readonly ICustomerRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public GetCustomerListQueryHandler(ICustomerRepository repository, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<PagedResult<CustomerListDto>> Handle(GetCustomerListQuery request, CancellationToken cancellationToken)
        {
            var req = request.Request;

            Guid? tenantId = null;
            if (Guid.TryParse(_currentUserService.UserGuid, out var tenantGuid))
                tenantId = tenantGuid;

            var pagedResult = await _repository.GetPagedAsync(
                req.PageIndex,
                req.PageSize,
                tenantId,
                req.Search,
                req.Filters?.StatusId,
                req.Filters?.TierId,
                req.Filters?.MinTotalSpent,
                req.Filters?.MaxTotalSpent,
                req.Filters?.MinPoints,
                req.Filters?.MaxPoints,
                req.Sort?.Field ?? "full_name",
                req.Sort?.Descending ?? false);

            var dtos = pagedResult.Items.Select(c => new CustomerListDto
            {
                id = c.id,
                legacy_id = c.legacy_id,
                full_name = c.full_name ?? string.Empty,
                phone_number = c.phone_number,
                email = c.email,
                current_points_balance = c.current_points_balance ?? 0,
                total_spent_amount = c.total_spent_amount ?? 0,
                tier_id = c.tier_id,
                tier_name = c.loyalty_tier?.tier_name,
                status_id = c.status_id ?? 0,
                status_code = c.status?.code,
                status_name = c.status != null
                    ? (c.status.translations?.FirstOrDefault(t => t.locale_id == _currentUserService.LocaleId)?.name ??
                       $"{c.status.translations?.FirstOrDefault(t => t.locale_id == 2)?.name} ({c.status.translations?.FirstOrDefault(t => t.locale_id == 1)?.name})")
                    : null
            }).ToList();

            return new PagedResult<CustomerListDto>(dtos, pagedResult.TotalCount, pagedResult.CurrentPage, pagedResult.PageSize);
        }
    }
}
