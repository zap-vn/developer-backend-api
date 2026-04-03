using MediatR;
using Microsoft.EntityFrameworkCore;
using CRM.BuildingBlocks.Models;
using CRM.Customer.Application.Features.Memberships.DTOs;
using CRM.Customer.Application.Features.Memberships.Queries;
using CRM.Customer.Infrastructure.Persistence;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.Customer.Infrastructure.Persistence.Queries
{
    public class GetMembershipListQueryHandler : IRequestHandler<GetMembershipListQuery, PagedResult<MembershipListDto>>
    {
        private readonly PostgresDbContext _context;

        public GetMembershipListQueryHandler(PostgresDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<MembershipListDto>> Handle(GetMembershipListQuery request, CancellationToken cancellationToken)
        {
            var query = _context.MembershipSubscriptions
                .Include(s => s.customer)
                .Include(s => s.plan)
                .AsNoTracking();

            if (request.TenantId.HasValue)
            {
                query = query.Where(s => s.customer.tenant_id == request.TenantId.Value);
            }

            if (request.PlanId.HasValue)
            {
                query = query.Where(s => s.plan_id == request.PlanId.Value);
            }

            if (request.StatusId.HasValue)
            {
                query = query.Where(s => s.status_id == request.StatusId.Value);
            }

            if (request.IsExpired.HasValue)
            {
                var now = DateTime.UtcNow;
                if (request.IsExpired.Value)
                    query = query.Where(s => s.end_date.HasValue && s.end_date < now);
                else
                    query = query.Where(s => !s.end_date.HasValue || s.end_date >= now);
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var results = await query
                .OrderByDescending(s => s.start_date)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(s => new MembershipListDto
                {
                    id = s.id,
                    customer_name = s.customer.full_name ?? "Unknown",
                    plan_name = s.plan.plan_name,
                    start_date = s.start_date,
                    end_date = s.end_date,
                    auto_renew = s.auto_renew,
                    status_id = s.status_id
                })
                .ToListAsync(cancellationToken);

            return new PagedResult<MembershipListDto>(results, totalCount, request.Page, request.PageSize);
        }
    }
}
