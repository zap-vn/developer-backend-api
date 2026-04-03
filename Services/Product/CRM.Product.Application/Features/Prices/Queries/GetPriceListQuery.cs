using MediatR;
using CRM.BuildingBlocks.Models;
using CRM.Product.Application.Features.Prices.DTOs;
using System;

namespace CRM.Product.Application.Features.Prices.Queries
{
    public class GetPriceListQuery : IRequest<PagedResult<PriceListDto>>
    {
        public Guid? TenantId { get; set; }
        public Guid LocationId { get; set; } // Mandatory per design
        public Guid? CategoryId { get; set; }
        public string? SearchTerm { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
