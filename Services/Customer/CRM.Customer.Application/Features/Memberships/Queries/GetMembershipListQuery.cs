using MediatR;
using CRM.BuildingBlocks.Models;
using CRM.Customer.Application.Features.Memberships.DTOs;
using System;

namespace CRM.Customer.Application.Features.Memberships.Queries
{
    public class GetMembershipListQuery : IRequest<PagedResult<MembershipListDto>>
    {
        public Guid? TenantId { get; set; }
        public Guid? PlanId { get; set; }
        public int? StatusId { get; set; }
        public bool? IsExpired { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
