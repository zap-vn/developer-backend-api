using MediatR;
using System.Collections.Generic;
using CRM.Product.Application.Features.Collections.DTOs;
using CRM.BuildingBlocks.Domain.Common;

namespace CRM.Product.Application.Features.Collections.Queries
{
    public class GetCollectionListQuery : IRequest<PagedResult<CollectionDto>>
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
    }
}
