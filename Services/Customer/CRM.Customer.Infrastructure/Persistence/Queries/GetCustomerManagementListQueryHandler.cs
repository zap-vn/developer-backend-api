using MediatR;
using Microsoft.EntityFrameworkCore;
using CRM.BuildingBlocks.Models;
using CRM.Customer.Application.Features.Customers.DTOs;
using CRM.Customer.Application.Features.Customers.Queries;
using CRM.Customer.Infrastructure.Persistence;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.Customer.Infrastructure.Persistence.Queries
{
    public class GetCustomerManagementListQueryHandler : IRequestHandler<GetCustomerManagementListQuery, PagedResult<CustomerListDto>>
    {
        private readonly PostgresDbContext _context;

        public GetCustomerManagementListQueryHandler(PostgresDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<CustomerListDto>> Handle(GetCustomerManagementListQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Customers
                .Include(c => c.loyalty_tier)
                .AsNoTracking();

            if (request.TenantId.HasValue)
            {
                query = query.Where(c => c.tenant_id == request.TenantId.Value);
            }

            if (!string.IsNullOrEmpty(request.PhoneNumber))
            {
                query = query.Where(c => c.phone_number != null && c.phone_number.Contains(request.PhoneNumber));
            }

            if (!string.IsNullOrEmpty(request.Email))
            {
                query = query.Where(c => c.email != null && c.email.Contains(request.Email));
            }

            if (request.TierId.HasValue)
            {
                query = query.Where(c => c.tier_id == request.TierId.Value);
            }

            if (request.StatusId.HasValue)
            {
                query = query.Where(c => c.status_id == request.StatusId.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var results = await query
                .OrderByDescending(c => c.id)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new CustomerListDto
                {
                    id = c.id,
                    full_name = c.full_name ?? "Unknown",
                    phone_number = c.phone_number,
                    email = c.email,
                    current_points_balance = c.current_points_balance ?? 0,
                    total_spent_amount = c.total_spent_amount ?? 0,
                    tier_name = c.loyalty_tier != null ? c.loyalty_tier.tier_name : "N/A",
                    status_id = c.status_id ?? 0
                })
                .ToListAsync(cancellationToken);

            return new PagedResult<CustomerListDto>(results, totalCount, request.Page, request.PageSize);
        }
    }
}
